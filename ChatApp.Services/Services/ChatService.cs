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
        public Task<List<PrivateChatMessageDto>> GetHistoryMessage(long chatId, long userId)
        {
            var chatMessageRepository = _unitOfWork.GetRepository<ChatMessage>();

            return chatMessageRepository.GetQuery()
                .Where(p =>
                            (p.SenderId == userId || p.ReceiverId == userId) &&
                            (p.Chat.User1 == userId || p.Chat.User2 == userId) &&
                            p.ChatId == chatId &&
                            p.MessageType == MessageType.Message &&
                            p.ReceiverType == ReceiverType.Private)
                .Select(p => new PrivateChatMessageDto()
                {
                    ChatId = p.ChatId,
                    CreatedAt = p.CreatedAt,
                    ReceiverId = p.ReceiverId,
                    SenderId = p.SenderId,
                    Message = p.Message,
                    ChatMessageId = p.Id,
                    UpdatedAt = p.UpdatedAt,
                    ActiveUserHasSender = userId == p.SenderId
                })
                .ToListAsync();
        }
    }
}
