using System.Collections.Generic;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer;

public class UserRepository : IUserRepository
{
    public void CreateUser(User user)
    {
        var userRepo = FileHandlingService.ReadFile<HashSet<User>>(FilePathResource.UserRepoFilePath);
        userRepo.Add(user);
        FileHandlingService.WriteFile(userRepo, FilePathResource.UserRepoFilePath);
    }

    public IReadOnlySet<User> FetchAllUsers()
    {
        return FileHandlingService.ReadFile<HashSet<User>>(FilePathResource.UserRepoFilePath) ?? new HashSet<User>();       
    }
}
