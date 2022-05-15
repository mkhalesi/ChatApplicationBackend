using ChatApp.Entities.Common;

namespace ChatApp.Entities.Models.Chat
{
    public class Group : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Member> Members { get; set; }
    }
}
