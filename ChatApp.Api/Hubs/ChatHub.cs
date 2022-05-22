using ChatApp.DataAccess.UoW;
using ChatApp.Dtos.Models.Chats;
using ChatApp.Entities.Enums;
using ChatApp.Entities.Models.Chat;
using ChatApp.Services.IServices;
using ChatApp.Utilities.Constants;
using ChatApp.Utilities.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Hubs
{
    public class ChatHub : BaseHub
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public ChatHub(IUserService userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }
        public async Task SendMessage(SendMessageDTO message)
        {
            message.ReceiverId = 6;
            message.ChatId = 1;

            var chatMessageRepository = _unitOfWork.GetRepository<ChatMessage>();
            var newMessage = new ChatMessage()
            {
                SenderId = int.Parse(Context.GetHttpContext()?.UserId()),
                ReceiverId = message.ReceiverId,
                MessageType = MessageType.Message,
                ReceiverType = ReceiverType.Private,
                Message = message.Message,
                ChatId = 1,
            };
            await chatMessageRepository.AddEntity(newMessage);
            await chatMessageRepository.SaveChanges();

            var messageObject = new PrivateChatMessageDto()
            {
                SenderId = newMessage.SenderId,
                ChatId = newMessage.ChatId,
                Message = newMessage.Message,
                ReceiverId = newMessage.ReceiverId,
                ChatMessageId = newMessage.Id,
                CreatedAt = newMessage.CreatedAt.ToString("t")
            };

            await Clients.All.SendAsync(HubMethods.ReceiveMessage, messageObject);
        }
    }
}
