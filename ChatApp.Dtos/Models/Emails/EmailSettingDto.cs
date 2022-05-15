namespace ChatApp.Dtos.Models.Emails
{
    public class EmailSettingDto
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }
    }
}
