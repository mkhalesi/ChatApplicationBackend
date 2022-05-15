using ChatApp.DataAccess.UoW;
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
        public Task<List<ChatMessage>> GetHistoryMessage(long chatId, long userId)
        {
            var chatMessageRepository = _unitOfWork.GetRepository<ChatMessage>();

            return chatMessageRepository.GetQuery()
                .Where(p =>
                            (p.SenderId == userId || p.ReceiverId == userId) &&
                            (p.Chat.User1 == userId || p.Chat.User2 == userId) &&
                            p.ChatId == chatId &&
                            p.MessageType == MessageType.Message &&
                            p.ReceiverType == ReceiverType.Private)
                .ToListAsync();
        }
    }
}
