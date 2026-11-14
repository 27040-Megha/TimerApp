using System.Collections.Generic;
using System.Linq;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer
{
    public class UserRepository : IUserRepository
    {
        public void CreateUser(User user)
        {
            var userRepo = FileHandlingService.ReadFile<User>(FilePathResource.UserRepoFilePath);
            userRepo.Add(user);
            FileHandlingService.WriteFile(userRepo, FilePathResource.UserRepoFilePath);
        }

        public IEnumerable<User> FetchAllUsers()
        {
            var userRepo = FileHandlingService.ReadFile<User>(FilePathResource.UserRepoFilePath);
            return userRepo.Select(user => new User(user.UserId, user.UserName, user.Email, user.Password, user.Activities));
        }
    }
}
