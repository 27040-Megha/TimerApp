using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Interface;

public interface ISessionManager
{
    public User? CurrentSessionUser { get; set; }
}
