using ChatApp.DataAccess.UoW;
using ChatApp.Dtos.Models.Chats;
using ChatApp.Entities.Enums;
using ChatApp.Entities.Models.Chat;
using ChatApp.Entities.Models.User;
using ChatApp.Services.IServices;
using ChatApp.Utilities.Constants;
using ChatApp.Utilities.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Api.Hubs
{
    public class ChatHub : BaseHub
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatService _chatService;
        public ChatHub(IUserService userService, IUnitOfWork unitOfWork, IChatService chatService)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _chatService = chatService;
        }
        public async Task SendMessage(SendMessageDTO message)
        {
            var userId = int.Parse(Context.GetHttpContext()?.UserId() ??
                                   throw new InvalidOperationException("User Not Found"));

            var userRepository = _unitOfWork.GetRepository<User>();
            var chatRepository = _unitOfWork.GetRepository<Chat>();
            var chatMessageRepository = _unitOfWork.GetRepository<ChatMessage>();

            if (!await userRepository.GetQuery().AnyAsync(p => p.Id == message.ReceiverId && p.Id != userId && !p.IsDeleted))
                throw new HubException("ReceiverId Not Found");
            if (string.IsNullOrWhiteSpace(message.Message))
                throw new HubException("Message cannot be Null");

            var chatExist = await chatRepository.GetEntityById(message.ChatId);
            if (chatExist == null)
            {
                var newUserChat = new Chat()
                {
                    User1 = userId,
                    User2 = message.ReceiverId,
                };
                await chatRepository.AddEntity(newUserChat);
                await chatRepository.SaveChanges();
            }

            var newMessage = new ChatMessage()
            {
                SenderId = userId,
                ReceiverId = message.ReceiverId,
                MessageType = MessageType.Message,
                ReceiverType = ReceiverType.Private,
                Message = message.Message,
                ChatId = message.ChatId,
                ReplyToMessageId = message.ReplyToMessageId != 0 ? message.ReplyToMessageId : null,
            };
            await chatMessageRepository.AddEntity(newMessage);
            await chatMessageRepository.SaveChanges();

            var newMessageFormDb = await chatMessageRepository.GetQuery()
                .Include(p => p.ReplyToMessage)
                .FirstOrDefaultAsync(p => p.Id == newMessage.Id);
            if (newMessageFormDb == null) throw new HubException("new message not Found From DB");

            var messageObject = new PrivateChatMessageDto()
            {
                SenderId = newMessageFormDb.SenderId,
                ChatId = newMessageFormDb.ChatId,
                Message = newMessageFormDb.Message,
                ReceiverId = newMessageFormDb.ReceiverId,
                ChatMessageId = newMessageFormDb.Id,
                CreatedAt = newMessageFormDb.CreatedAt.ToString("t"),
                ReplyToMessage = newMessageFormDb.ReplyToMessageId.HasValue && newMessageFormDb.ReplyToMessage != null ?
                    new ReplyToMessageDTO()
                    {
                        ReplyToMessageId = newMessageFormDb.ReplyToMessageId.Value,
                        ReplyToFullName = ChatExtensions.GetUserFullName(
                            userRepository.GetQuery().FirstOrDefault(s => s.Id == newMessageFormDb.ReplyToMessage.SenderId)),
                        Message = newMessageFormDb.ReplyToMessage.Message,
                        ReplyToUserId = newMessageFormDb.ReplyToMessage.SenderId,
                    } : null,
                ReadMessage = newMessageFormDb.ReadTime != null,
                SenderFullName = ChatExtensions.GetUserFullName(userRepository.GetQuery().FirstOrDefault(u => u.Id == newMessage.SenderId)),
            };

            await Clients.All.SendAsync(HubMethods.ReceiveMessage, messageObject);
        }

        public async Task ReceiverSeenMessages(long chatId)
        {
            var userId = int.Parse(Context.GetHttpContext()?.UserId() ??
                                   throw new InvalidOperationException("User Not Found"));
            var res = await _chatService.ReceiverSeenAllMessages(userId, chatId);
            await Clients.All.SendAsync(HubMethods.UpdateSenderMessagesReadTime, res);
        }
    }
}
