using System.Collections.Generic;
using System.Linq;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Service
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public bool CreateUser(User newUser)
        {
            var existingUsers = this.FetchAllUsers();
            var duplicateUser = existingUsers.Any(u => string.Equals(u.Email, newUser.Email));
            if (duplicateUser)
            {
                return false;
            }

            this._userRepository.CreateUser(newUser);
            return true;
        }

        public IEnumerable<User> FetchAllUsers()
        {
            return this._userRepository.FetchAllUsers();
        }
    }
}
