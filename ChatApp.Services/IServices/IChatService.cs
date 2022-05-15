
using ChatApp.Entities.Models.Chat;

namespace ChatApp.Services.IServices
{
    public interface IChatService
    {
       Task<List<ChatMessage>> GetHistoryMessage(long chatId ,  long userId);
    }
}
