
namespace ChatApp.Dtos.Models.Chats
{
    public class ChatDTO
    {
        public long ChatId { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public string LatestMessageText { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public string StartedChatDate { get; set; } 
        public string LastUpdatedChatDate { get; set; } 

    }
}
