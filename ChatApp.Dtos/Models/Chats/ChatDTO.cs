
namespace ChatApp.Dtos.Models.Chats
{
    public class ChatDTO
    {
        public long ChatId { get; set; }
        public long User1 { get; set; }
        public long User2 { get; set; }
        public string ReceiverFullName { get; set; }
        public string StartedChatDate { get; set; }
        public string LastUpdatedChatDate { get; set; } 

    }
}
