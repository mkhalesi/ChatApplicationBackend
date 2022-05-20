using ChatApp.Api.Filters;
using ChatApp.Api.Hubs;
using ChatApp.Dtos.Common;
using ChatApp.Dtos.Models.Chats;
using ChatApp.Entities.Models.Chat;
using ChatApp.Services.IServices;
using ChatApp.Utilities.Constants;
using ChatApp.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Controllers
{
    [Route("api/chat")]
    public class ChatController : BaseApiController
    {
        private readonly IChatService _chatService;
        private IHubContext<ChatHub> _chatHub;
        public ChatController(IChatService chatService, IHubContext<ChatHub> chatHub)
        {
            _chatService = chatService;
            _chatHub = chatHub;
        }

        [ServiceFilter(typeof(AuthFilter))]
        [HttpGet("GetReceiverMessage")]
        public async Task<IActionResult> GetReceiverMessage()
        {
            await _chatHub.Clients.All.SendAsync(HubMethods.HistoryMessages, GetHistoryMessages(1));
            return Ok(new
            {
                message = "Request Completed",
            });
        }

        [ServiceFilter(typeof(AuthFilter))]
        [HttpGet]
        [Route("HistoryMessages/{chatId}")]
        public async Task<BaseResponseDto<List<PrivateChatMessageDto>>> GetHistoryMessages(long chatId)
        {
            var userId = ControllerContext.HttpContext.UserId();

            if (string.IsNullOrEmpty(userId))
                return new BaseResponseDto<List<PrivateChatMessageDto>>()
                    .GenerateFailedResponse("Error", "not Authorized");

            if (chatId == 0)
                return new BaseResponseDto<List<PrivateChatMessageDto>>()
                    .GenerateFailedResponse("Error", "chatId is null");

            var result = await _chatService.GetHistoryMessage(chatId, int.Parse(userId));
            return new BaseResponseDto<List<PrivateChatMessageDto>>()
                .GenerateSuccessResponse(result);
        }
    }
}
