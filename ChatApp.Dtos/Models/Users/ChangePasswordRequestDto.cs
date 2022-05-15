namespace ChatApp.Dtos.Models.Users
{
    public class ChangePasswordRequestDto
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
