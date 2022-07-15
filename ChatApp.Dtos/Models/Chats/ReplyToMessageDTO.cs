namespace ChatApp.Dtos.Models.Chats
{
    public class ReplyToMessageDTO
    {
        public long ReplyToMessageId { get; set; }      
        public string? ReplyToFullName { get; set; }
        public string Message { get; set; }
    }
}
