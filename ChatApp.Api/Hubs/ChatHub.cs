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
        public async Task SendMessage(PrivateChatMessageDto privateChatMessage)
        {
            var chatMessageRepository = _unitOfWork.GetRepository<ChatMessage>();
            await chatMessageRepository.AddEntity(new ChatMessage()
            {
                SenderId = int.Parse(Context.GetHttpContext()?.UserId()),
                ReceiverId = privateChatMessage.ReceiverId,
                MessageType = MessageType.Message,
                ReceiverType = ReceiverType.Private,
                Message = privateChatMessage.Content,
            });
            await chatMessageRepository.SaveChanges();

            await Clients.All.SendAsync(HubMethods.ReceiveMessage, privateChatMessage);
        }
    }
}
