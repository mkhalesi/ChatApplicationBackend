using ChatApp.Dtos.Models.Chats;
using ChatApp.Entities.Models.User;

namespace ChatApp.Utilities.Extensions
{
    public static class ChatExtensions
    {
        public static string? GetUserFullName(User? user)
        {
            if (user == null) return null;
            return user.FirstName + " " + user.LastName;
        }
    }
}
