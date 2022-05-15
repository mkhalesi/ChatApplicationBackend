using System.ComponentModel.DataAnnotations;

namespace ChatApp.Entities.Models.Chat;

public class Member
{
    [Key]
    public string UserId { get; set; }

    public string NickName { get; set; }

    public DateTime JoinedAt { get; set; }
}