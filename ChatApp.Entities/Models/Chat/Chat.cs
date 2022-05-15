using ChatApp.Entities.Common;

namespace ChatApp.Entities.Models.Chat
{
    public class Chat : BaseEntity
    {
        public long User1 { get; set; }
        public long User2 { get; set; }


        #region relations

        public ICollection<ChatMessage> ChatMessages { get; set; }

        public ICollection<User.User> Users { get; set; }

        #endregion
    }
}
