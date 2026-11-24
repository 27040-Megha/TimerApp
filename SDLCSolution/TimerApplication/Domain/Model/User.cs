using System;

namespace TimerApplication.Domain.Model;

public class User
{
    public User()
    {
    }

    public User(string userName, string email, string password)
    {
        this.UserId = Guid.NewGuid();
        this.UserName = userName;
        this.Email = email;
        this.Password = password;
    }

    public User(Guid userId, string userName, string email, string password)
    {
        this.UserId = userId;
        this.UserName = userName;
        this.Email = email;
        this.Password = password;
    }

    public Guid UserId { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
}
