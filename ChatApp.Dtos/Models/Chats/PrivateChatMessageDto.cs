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
        public string UpdatedAt { get; set; }
        public string CreatedAt { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public ReplyToMessageDTO? ReplyToMessage { get; set; }
        public bool ReadMessage { get; set; }
    }
}
