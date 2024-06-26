﻿namespace ChatApp.Dtos.Models.Users
{
    public class UserDto
    {
        public long Id { get; set; }

        public long ChatId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int Role { get; set; }

        public string GooglePassword { get; set; }
    }
}
