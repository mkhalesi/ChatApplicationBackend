namespace ChatApp.Dtos.Models.Users
{
    public class UserResponseDto
    {
        public long Id{ get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int Role { get; set; }

        public bool IsConfirmed { get; set; }
    }
}
