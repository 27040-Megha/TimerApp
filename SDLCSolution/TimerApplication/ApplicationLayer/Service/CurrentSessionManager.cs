using System;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Service
{
    public static class CurrentSessionManager
    {
        public static User CurrentSessionUser{ get; set; }
    }
}
