using ChatApp.Entities.Common;
using ChatApp.Entities.Enums;

namespace ChatApp.Entities.Models.Chat
{
    public class ChatMessage : BaseEntity
    {
        public long ChatId { get; set; }
        public long SenderId { get; set; }

        public long ReceiverId { get; set; }

        public ReceiverType ReceiverType { get; set; }

        public MessageType MessageType { get; set; }

        public string Message { get; set; }

        #region relations

        public Chat Chat { get; set; }

        #endregion
    }
}
