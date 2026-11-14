using System.Collections.Generic;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Interface
{
    public interface IUserRepository
    {
        public void CreateUser(User user);

        public IEnumerable<User> FetchAllUsers();
    }
}
