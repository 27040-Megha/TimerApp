using TimerApplication.ApplicationLayer.Interface;

namespace TimerApplication.InfrastructureLayer;

public class ActiveUserRepository : IActiveUserRepository
{
    public Guid? FetchActiveUser()
    {
        return FileHandlingService.ReadFile<Guid?>(FilePathResource.LastActiveUser);
    }

    public void SaveCurrentActiveUser(Guid? userId)
    {
        FileHandlingService.WriteFile(userId, FilePathResource.LastActiveUser);
    }
}
