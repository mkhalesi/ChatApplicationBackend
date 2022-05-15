using ChatApp.Entities.Common;

namespace ChatApp.Entities.Models.Access
{
    public class UserRole: BaseEntity
    {
        #region properties

        public long RoleId { get; set; }
        public long UserId { get; set; }

        #endregion


        #region relations

        public Role Role { get; set; }
        public User.User User { get; set; }

        #endregion
    }
}
