using System;
using System.Collections.Generic;

namespace TimerApplication.Domain.Model
{
    public class User
    {
        public User(Guid userId, string userName, string email, string password, List<Activity> activities)
        {
            this.UserId = userId;
            this.UserName = userName;
            this.Email = email;
            this.Password = password;
            this.Activities = activities;
        }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public List<Activity> Activities { get; set; }
    }
}
