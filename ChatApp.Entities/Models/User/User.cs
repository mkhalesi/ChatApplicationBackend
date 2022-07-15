using ChatApp.Entities.Common;
using ChatApp.Entities.Models.Access;
using ChatApp.Entities.Models.Chat;

namespace ChatApp.Entities.Models.User
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsConfirmed { get; set; }

        public string ConfirmationToken { get; set; }

        public string GooglePassword { get; set; }


        #region relations

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Chat.Chat> Chats { get; set; }

        #endregion
    }
}
