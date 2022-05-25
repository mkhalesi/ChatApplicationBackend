
namespace ChatApp.Dtos.Models.Chats
{
    public class ReceiverChatDTO
    {
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public long ChatId { get; set; }
    }
}
