using System;
using System.Collections.Generic;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Interface;

public interface IActivityLogRepository
{
    public void CreateActivityLog(ActivityLog activityLog);

    public void DeleteActivityLog(Guid activityLogId);

    public void EditActivityLog(Guid activityLogId, ActivityLog activityLog);

        public IEnumerable<ActivityLog> FetchAllActivityLogs();

        public void ExportActivityLogsToCsv(string fileName, List<ActivityLog> logsToExport);
    }
}
