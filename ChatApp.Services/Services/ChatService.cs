using ChatApp.DataAccess.UoW;
using ChatApp.Dtos.Models.Chats;
using ChatApp.Dtos.Models.Paging;
using ChatApp.Dtos.Models.Users;
using ChatApp.Entities.Enums;
using ChatApp.Entities.Models.Chat;
using ChatApp.Entities.Models.User;
using ChatApp.Services.IServices;
using ChatApp.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<ChatDTO>> GetAllUserChats(long userId)
        {
            var chatRepository = _unitOfWork.GetRepository<Chat>();
            var findReceiverInUserChats = GetUserReceiversFromCurrentUserChats(userId);

            var res = await chatRepository.GetQuery()
                .AsQueryable()
                .Include(p => p.Users)
                .Include(p => p.ChatMessages)
                .Where(p => p.User1 == userId || p.User2 == userId && !p.IsDeleted && p.ChatMessages.Any())
                .Select(p => new ChatDTO()
                {
                    ChatId = p.Id,
                    SenderId = p.User1 == userId ? p.User1 : p.User2,
                    ReceiverId = p.User1 == userId ? p.User2 : p.User1,
                    StartedChatDate = p.CreatedAt.ToString("M"),
                    LastUpdatedChatDate = p.UpdatedAt.Value.ToString("M"),
                    ReceiverFirstName = findReceiverInUserChats.Any(s => s.ChatId == p.Id) ?
                        findReceiverInUserChats.Single(s => s.ChatId == p.Id).FirstName : "",
                    ReceiverLastName = findReceiverInUserChats.Any(s => s.ChatId == p.Id) ?
                        findReceiverInUserChats.Single(s => s.ChatId == p.Id).LastName : "",
                    LatestMessageText = p.ChatMessages.Any() ?
                        p.ChatMessages.OrderByDescending(s => s.CreatedAt).First().Message : "",
                    UnreadMessagesCount = p.ChatMessages.Count(s => s.ReadTime == null && s.ReceiverId == userId)
                })
                .ToListAsync();

            return res.OrderByDescending(p => DateTime.Parse(p.LastUpdatedChatDate)).ToList();
        }

        public async Task<ChatDTO> GetUserChatByChatId(long userId, long chatId)
        {
            var chatRepository = _unitOfWork.GetRepository<Chat>();
            var findReceiverInUserChats = GetUserReceiversFromCurrentUserChats(userId);

            return await chatRepository.GetQuery()
                .Where(p => (p.User1 == userId || p.User2 == userId)
                            && p.Id == chatId)
                .Select(p => new ChatDTO()
                {
                    ChatId = p.Id,
                    SenderId = p.User1 == userId ? p.User1 : p.User2,
                    ReceiverId = p.User1 == userId ? p.User2 : p.User1,
                    StartedChatDate = p.CreatedAt.ToString("M"),
                    LastUpdatedChatDate = p.CreatedAt.ToString("M"),
                    ReceiverFirstName = findReceiverInUserChats.Any(s => s.ChatId == p.Id) ?
                        findReceiverInUserChats.First(s => s.ChatId == p.Id).FirstName : "",
                    ReceiverLastName = findReceiverInUserChats.Any(s => s.ChatId == p.Id) ?
                        findReceiverInUserChats.First(s => s.ChatId == p.Id).LastName : "",
                    LatestMessageText = p.ChatMessages.Any() ?
                        p.ChatMessages.OrderByDescending(s => s.CreatedAt).First().Message : "",
                }).FirstOrDefaultAsync()
                   ?? throw new InvalidOperationException();
        }

        public async Task<FilterPrivateMessagesDTO> GetHistoryMessage(FilterPrivateMessagesDTO filter)
        {
            var chatMessageRepository = _unitOfWork.GetRepository<ChatMessage>();
            var userRepository = _unitOfWork.GetRepository<User>();
            var chatMessagesQuery = chatMessageRepository.GetQuery()
                .Where(p =>
                    (p.SenderId == filter.UserId || p.ReceiverId == filter.UserId) &&
                    (p.Chat.User1 == filter.UserId || p.Chat.User2 == filter.UserId) &&
                    p.ChatId == filter.ChatId &&
                    p.MessageType == MessageType.Message &&
                    p.ReceiverType == ReceiverType.Private)
                .AsQueryable();

            chatMessagesQuery = chatMessagesQuery.OrderByDescending(p => p.CreatedAt).AsQueryable();

            var count = (int)Math.Ceiling(chatMessagesQuery.Count() / (double)filter.TakeEntity);
            var pager = Pager.Build(count, filter.PageId, filter.TakeEntity);

            var messages = await chatMessagesQuery
                .Paging(pager)
                .Select(p => new PrivateChatMessageDto()
                {
                    ChatId = p.ChatId,
                    CreatedAt = p.CreatedAt.ToString("t"),
                    ReceiverId = p.ReceiverId,
                    SenderId = p.SenderId,
                    Message = p.Message,
                    ChatMessageId = p.Id,
                    UpdatedAt = p.UpdatedAt.Value.ToString("t"),
                    ActiveUserHasSender = filter.UserId == p.SenderId,
                    CreatedDateTime = p.CreatedAt,
                    ReplyToMessage = p.ReplyToMessageId.HasValue && p.ReplyToMessage != null ?
                        new ReplyToMessageDTO()
                        {
                            ReplyToMessageId = p.ReplyToMessageId.Value,
                            ReplyToFullName = ChatExtensions.GetUserFullName(
                                userRepository.GetQuery().FirstOrDefault(s => s.Id == p.ReplyToMessage.SenderId)),
                            Message = p.ReplyToMessage.Message,
                            ReplyToUserId = p.ReplyToMessage.SenderId,
                        } : null,
                    ReadMessage = p.ReadTime.HasValue
                }).ToListAsync();

            var res = filter.SetChatMessages(messages.OrderBy(p => p.CreatedDateTime)
                                            .ToList()).SetPaging(pager);
            return res;
        }

        public async Task<bool> SeenMessages(long userId, long chatId)
        {
            var chatMessageRepo = _unitOfWork.GetRepository<ChatMessage>();

            var findUnreadMessages = await chatMessageRepo.GetQuery()
                .Where(p => p.ReadTime == null && p.ChatId == chatId && p.ReceiverId == userId)
                .ToListAsync();
            if (findUnreadMessages.Any())
            {
                foreach (var unMessage in findUnreadMessages)
                {
                    unMessage.ReadTime = DateTime.Now;
                }
                await chatMessageRepo.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> ReceiverSeenAllMessages(long userId, long chatId)
        {
            var chatRepo = _unitOfWork.GetRepository<Chat>();
            var chatMessageRepo = _unitOfWork.GetRepository<ChatMessage>();
            int receiverId;
            var chatFromDb = await chatRepo.GetQuery().FirstOrDefaultAsync(p => p.Id == chatId);
            if (chatFromDb == null)
                return false;

            if (chatFromDb.User1 == userId)
                receiverId = (int)chatFromDb.User2;
            else
                receiverId = (int)chatFromDb.User1;

            var findMessages = chatMessageRepo.GetQuery()
                .Where(p => p.ChatId == chatId && p.ReceiverId == receiverId && p.SenderId == userId)
                .AsQueryable();
            var findUnreadMessages = findMessages.Where(p => p.ReadTime == null);
            if (!findUnreadMessages.Any() && findMessages.Any())
                return true;

            return false;
        }

        #region helpers

        private IQueryable<UserDto> GetUserReceiversFromCurrentUserChats(long userId)
        {
            var chatRepository = _unitOfWork.GetRepository<Chat>();
            var userRepository = _unitOfWork.GetRepository<User>();

            var userChats = chatRepository.GetQuery().AsQueryable()
                .Where(p => p.User1 == userId || p.User2 == userId && !p.IsDeleted)
                .Select(p => new ReceiverChatDTO()
                {
                    SenderId = p.User1 == userId ? p.User1 : p.User2,
                    ReceiverId = p.User1 == userId ? p.User2 : p.User1,
                    ChatId = p.Id,
                })
                .AsQueryable();
            return userRepository.GetQuery().AsQueryable()
                .Where(p => userChats.Select(s => s.ReceiverId).Contains(p.Id))
                .Select(p => new UserDto()
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    ChatId = userChats.SingleOrDefault(s => s.ReceiverId == p.Id)!.ChatId,
                }).AsQueryable();
        }

        public string GetSenderFullNameByNewMessage(PrivateChatMessageDto newMessage)
        {
            var userRepository = _unitOfWork.GetRepository<User>();
            var findSenderFromDb = userRepository.GetQuery().FirstOrDefault(p => p.Id == newMessage.SenderId);
            if (findSenderFromDb == null) return "";
            return findSenderFromDb.FirstName + " " + findSenderFromDb.LastName;
        }

        #endregion
    }
}