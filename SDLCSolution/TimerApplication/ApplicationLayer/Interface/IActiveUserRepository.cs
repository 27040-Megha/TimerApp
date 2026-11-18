using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Interface;

public interface IActiveUserRepository
{
    public void SaveCurrentActiveUser(User user);

    public User FetchActiveUser();
}
