using ChatApp.Dtos.Models.Chats;

namespace ChatApp.Services.IServices
{
    public interface IChatService
    {
        Task<List<ChatDTO>> GetAllUserChats(long userId);
        Task<ChatDTO> GetUserChatByChatId(long userId, long chatId);
        Task<FilterPrivateMessagesDTO> GetHistoryMessage(FilterPrivateMessagesDTO filter);
        Task<bool> SeenMessages(long userId, long chatId);
        Task<bool> ReceiverSeenAllMessages(long userId, long chatId);
    }
}
