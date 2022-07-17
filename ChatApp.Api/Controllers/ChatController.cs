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
        [HttpGet("HistoryMessages")]
        public async Task<BaseResponseDto<FilterPrivateMessagesDTO>> GetHistoryMessages([FromQuery] FilterPrivateMessagesDTO filter)
        {
            var userId = ControllerContext.HttpContext.UserId();

            if (string.IsNullOrEmpty(userId))
                return new BaseResponseDto<FilterPrivateMessagesDTO>()
                    .GenerateFailedResponse("Error", "not Authorized");

            if (filter.ChatId == 0)
                return new BaseResponseDto<FilterPrivateMessagesDTO>()
                    .GenerateFailedResponse("Error", "chatId is null");

            filter.UserId = int.Parse(userId);
            filter.TakeEntity = filter.TakeEntity != 0 ? filter.TakeEntity : 15;

            var result = await _chatService.GetHistoryMessage(filter);
            return new BaseResponseDto<FilterPrivateMessagesDTO>()
                .GenerateSuccessResponse(result);
        }

        [ServiceFilter(typeof(AuthFilter))]
        [HttpGet("SeenMessages/{chatId}")]
        public async Task<BaseResponseDto<bool>> SeenMessages(long chatId)
        {
            var userId = ControllerContext.HttpContext.UserId();
            if (string.IsNullOrEmpty(userId))
                return new BaseResponseDto<bool>().GenerateFailedResponse(ErrorCodes.Unauthorized);
            if (chatId == 0)
                return new BaseResponseDto<bool>().GenerateFailedResponse(ErrorCodes.Forbidden);

            var res = await _chatService.SeenMessages(int.Parse(userId), chatId);
            return new BaseResponseDto<bool>().GenerateSuccessResponse(res);
        }
    }
}
