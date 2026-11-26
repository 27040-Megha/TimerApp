using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Service;

public class CurrentSessionManager : ISessionManager
{
    public  User? CurrentSessionUser{ get; set; }
}
