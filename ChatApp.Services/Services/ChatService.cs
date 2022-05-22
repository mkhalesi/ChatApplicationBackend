using ChatApp.DataAccess.UoW;
using ChatApp.Dtos.Models.Chats;
using ChatApp.Entities.Enums;
using ChatApp.Entities.Models.Chat;
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
            var res = await chatRepository.GetQuery()
                .Include(p => p.Users)
                .Where(p => p.User1 == userId || p.User2 == userId && !p.IsDeleted && p.ChatMessages.Any())
                .Select(p => new ChatDTO()
                {
                    ChatId = p.Id,
                    User1 = p.User1,
                    User2 = p.User2,
                    StartedChatDate = p.CreatedAt.ToString("M"),
                    LastUpdatedChatDate = p.CreatedAt.ToString("M"),
                    ReceiverFullName = p.Users.FirstOrDefault(s => s.Id == userId) != null ?
                        p.Users.FirstOrDefault(s => s.Id != userId).FirstName + " " +
                        p.Users.FirstOrDefault(s => s.Id != userId).LastName : "",
                })
                .ToListAsync();

            return res.OrderByDescending(p => DateTime.Parse(p.LastUpdatedChatDate)).ToList();
        }

        public async Task<ChatDTO> GetUserChatByChatId(long userId, long chatId)
        {
            var chatRepository = _unitOfWork.GetRepository<Chat>();

            return await chatRepository.GetQuery()
                .Where(p => (p.User1 == userId || p.User2 == userId)
                            && p.Id == chatId)
                .Select(p => new ChatDTO()
                {
                    ChatId = p.Id,
                    User1 = p.User1,
                    User2 = p.User2,
                    StartedChatDate = p.CreatedAt.ToString("M"),
                    LastUpdatedChatDate = p.CreatedAt.ToString("M"),
                    ReceiverFullName = p.Users.FirstOrDefault(s => s.Id == userId) != null ?
                        p.Users.FirstOrDefault(s => s.Id != userId).FirstName + " " +
                        p.Users.FirstOrDefault(s => s.Id != userId).LastName : "",
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
    }
}
