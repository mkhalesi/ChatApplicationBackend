namespace ChatApp.Dtos.Models.Chats
{
    public class PrivateChatMessageDto
    {
        public long ChatId { get; set; }
        public long ChatMessageId { get; set; }
        public long ReceiverId { get; set; }
        public long SenderId { get; set; }
        public string Message { get; set; }
        public bool ActiveUserHasSender { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
