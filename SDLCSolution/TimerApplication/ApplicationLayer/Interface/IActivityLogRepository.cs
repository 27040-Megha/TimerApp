using System;
using System.Collections.Generic;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Interface;

public interface IActivityLogRepository
{
    public void CreateActivityLog(ActivityLog activityLog);

    public void DeleteActivityLog(Guid activityLogId);

    public void EditActivityLog(Guid activityLogId, ActivityLog activityLog);
    void ExportActivityLogsToCsv(string v, List<ActivityLog> logsToExport);
    public IEnumerable<ActivityLog> FetchAllActivityLogs();
    List<ActivityLog> ImportActivityLogsFromCsv(string filePath, Guid activityId);
}
