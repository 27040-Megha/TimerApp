using System;
using System.Collections.Generic;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Interface;

public interface IActivityLogRepository
{
    public void CreateActivityLog(ActivityLog activityLog);

    public void EditActivityLog(Guid activityLogId, ActivityLog activityLog);

    public IEnumerable<ActivityLog> FetchAllActivityLogs();
}
