using ChatApp.DataAccess.UoW;
using ChatApp.Dtos.Models.Chats;
using ChatApp.Dtos.Models.Users;
using ChatApp.Entities.Enums;
using ChatApp.Entities.Models.Chat;
using ChatApp.Entities.Models.User;
using ChatApp.Services.IServices;
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
            var findReceiverInUserChats = await GetUserReceiversFromCurrentUserChats(userId);

            var res = await chatRepository.GetQuery()
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
                    /*ReceiverFirstName = findReceiverInUserChats.FirstOrDefault(s => s.Id == userId).FirstName,
                    ReceiverLastName = findReceiverInUserChats.FirstOrDefault(s => s.Id == userId).LastName,*/
                    LatestMessageText = p.ChatMessages.Any() ?
                        p.ChatMessages.OrderByDescending(s => s.CreatedAt).First().Message : "",
                })
                .ToListAsync();

            return res.OrderByDescending(p => DateTime.Parse(p.LastUpdatedChatDate)).ToList();
        }

        public async Task<ChatDTO> GetUserChatByChatId(long userId, long chatId)
        {
            var chatRepository = _unitOfWork.GetRepository<Chat>();
            var findReceiverInUserChats = await GetUserReceiversFromCurrentUserChats(userId);

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
                    /*ReceiverFirstName = p.User1 == userId ?
                            findReceiverInUserChats.First(s => s.Id == p.User2).FirstName :
                            p.User2 == userId ?
                                findReceiverInUserChats.First(s => s.Id == p.User1).FirstName : "",
                    ReceiverLastName = p.User1 == userId ?
                        findReceiverInUserChats.First(s => s.Id == p.User2).LastName :
                        p.User2 == userId ?
                            findReceiverInUserChats.First(s => s.Id == p.User1).LastName : "",*/
                }).FirstOrDefaultAsync()
                   ?? throw new InvalidOperationException();
        }

        public async Task<List<PrivateChatMessageDto>> GetHistoryMessage(long chatId, long userId)
        {
            var chatMessageRepository = _unitOfWork.GetRepository<ChatMessage>();

            var result = await chatMessageRepository.GetQuery()
                .Where(p =>
                    (p.SenderId == userId || p.ReceiverId == userId) &&
                    (p.Chat.User1 == userId || p.Chat.User2 == userId) &&
                    p.ChatId == chatId &&
                    p.MessageType == MessageType.Message &&
                    p.ReceiverType == ReceiverType.Private)
                .Select(p => new PrivateChatMessageDto()
                {
                    ChatId = p.ChatId,
                    CreatedAt = p.CreatedAt.ToString("M"),
                    ReceiverId = p.ReceiverId,
                    SenderId = p.SenderId,
                    Message = p.Message,
                    ChatMessageId = p.Id,
                    UpdatedAt = p.UpdatedAt.Value.ToString("M"),
                    ActiveUserHasSender = userId == p.SenderId
                })
                .ToListAsync();

            return result.OrderBy(p => p.CreatedAt).ToList();

        }


        #region helpers

        private async Task<List<UserDto>> GetUserReceiversFromCurrentUserChats(long userId)
        {
            var chatRepository = _unitOfWork.GetRepository<Chat>();
            var userRepository = _unitOfWork.GetRepository<User>();

            var userChats = await chatRepository.GetQuery()
                .Where(p => p.User1 == userId || p.User2 == userId && !p.IsDeleted)
                .ToListAsync();
            return await userRepository.GetQuery()
                .Where(p => userChats.Select(s => s.User1).Contains(p.Id) || userChats.Select(s => s.User2).Contains(p.Id))
                .Select(p => new UserDto()
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                }).ToListAsync();
        }

        #endregion
    }
}
