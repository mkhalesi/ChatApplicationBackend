using ChatApp.Dtos.Common;
using ChatApp.Entities.Models.Chat;
using ChatApp.Services.IServices;
using ChatApp.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers
{
    [Route("api/chat")]
    public class ChatController : BaseApiController
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        [Route("HistoryMessages/{chatId}")]
        public async Task<BaseResponseDto<List<ChatMessage>>> GetHistoryMessages(long chatId)
        {
            var userId = ControllerContext.HttpContext.UserId();

            if (User.Identity != null && (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId)))
                return new BaseResponseDto<List<ChatMessage>>()
                    .GenerateFailedResponse("Error", "not Authorized");

            if (chatId == 0)
                return new BaseResponseDto<List<ChatMessage>>()
                    .GenerateFailedResponse("Error", "chatId is null");

            var result = await _chatService.GetHistoryMessage(chatId, int.Parse(userId));
            return new BaseResponseDto<List<ChatMessage>>()
                .GenerateSuccessResponse(result);
        }
    }
}
