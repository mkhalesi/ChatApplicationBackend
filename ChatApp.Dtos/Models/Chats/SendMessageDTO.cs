namespace ChatApp.Dtos.Models.Chats
{
    public class SendMessageDTO
    {
        public long ReceiverId { get; set; }
        public long ChatId { get; set; }
        public string Message { get; set; }
    }
}
