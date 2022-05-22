
using ChatApp.Dtos.Models.Chats;
using ChatApp.Entities.Models.Chat;

namespace ChatApp.Services.IServices
{
    public interface IChatService
    {
        Task<List<ChatDTO>> GetAllUserChats(long userId);
        Task<ChatDTO> GetUserChatByChatId(long userId , long chatId);
        Task<List<PrivateChatMessageDto>> GetHistoryMessage(long chatId, long userId);
    }
}
