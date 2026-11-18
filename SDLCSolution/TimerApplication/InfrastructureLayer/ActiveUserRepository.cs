using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer
{
    public class ActiveUserRepository : IActiveUserRepository
    {
        public User FetchActiveUser()
        {
            return FileHandlingService.ReadFile<User>(FilePathResource.LastActiveUser);
        }

        public void SaveCurrentActiveUser(User user)
        {
            FileHandlingService.WriteFile(user, FilePathResource.LastActiveUser);
        }
    }
}
