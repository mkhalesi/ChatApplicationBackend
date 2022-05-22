using ChatApp.Api.Filters;
using ChatApp.Dtos.Common;
using ChatApp.Dtos.Models.Chats;
using ChatApp.Services.IServices;
using ChatApp.Utilities.Constants;
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

        /*
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
        */

        [ServiceFilter(typeof(AuthFilter))]
        [HttpGet("getAllUserChats")]
        public async Task<BaseResponseDto<List<ChatDTO>>> GetAllUserChats()
        {
            var userId = ControllerContext.HttpContext.UserId();
            if (string.IsNullOrEmpty(userId))
                return new BaseResponseDto<List<ChatDTO>>().GenerateFailedResponse(ErrorCodes.Unauthorized);

            var res = await _chatService.GetAllUserChats(int.Parse(userId));
            return new BaseResponseDto<List<ChatDTO>>()
                .GenerateSuccessResponse(res);
        }


        [ServiceFilter(typeof(AuthFilter))]
        [HttpGet("getUserChatByChatId/{chatId}")]
        public async Task<BaseResponseDto<ChatDTO>> GetUserChatByChatId(long chatId)
        {
            var userId = ControllerContext.HttpContext.UserId();
            if (string.IsNullOrEmpty(userId))
                return new BaseResponseDto<ChatDTO>().GenerateFailedResponse(ErrorCodes.Unauthorized);

            var res = await _chatService.GetUserChatByChatId(int.Parse(userId), chatId);
            return new BaseResponseDto<ChatDTO>()
                .GenerateSuccessResponse(res);
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
