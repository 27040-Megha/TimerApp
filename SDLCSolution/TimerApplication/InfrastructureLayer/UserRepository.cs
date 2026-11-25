using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer;

public class UserRepository : IUserRepository
{
    public void CreateUser(User user)
    {
        HashSet<User>? userRepo = FileHandlingService.ReadFile<HashSet<User>>(FilePathResource.UserRepoFilePath) ?? new HashSet<User>(); 
        userRepo?.Add(user);
        FileHandlingService.WriteFile(userRepo, FilePathResource.UserRepoFilePath);
    }

    public IReadOnlySet<User> FetchAllUsers()
    {
        return FileHandlingService.ReadFile<HashSet<User>>(FilePathResource.UserRepoFilePath) ?? new HashSet<User>();       
    }

    public User? FindByEmail(string? email)
    {
        HashSet<User>? userRepo = FileHandlingService.ReadFile<HashSet<User>>(FilePathResource.UserRepoFilePath);
        return userRepo?.FirstOrDefault(user => string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase));
    }

    public User? FindById(Guid? userId)
    {
        HashSet<User>? userRepo = FileHandlingService.ReadFile<HashSet<User>>(FilePathResource.UserRepoFilePath);
        return userRepo?.FirstOrDefault(user => string.Equals(user.UserId, userId));
    }
}
