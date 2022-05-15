using ChatApp.Entities.Common;

namespace ChatApp.Entities.Models.Chat
{
    public class FriendRequest : BaseEntity
    {
        public string FromUserId { get; set; }

        public string ToUserId { get; set; }

        public bool Accepted { get; set; }
    }
}
