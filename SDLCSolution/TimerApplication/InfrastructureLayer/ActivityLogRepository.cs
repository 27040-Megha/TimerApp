using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        public void CreateActivityLog(ActivityLog activityLog)
        {
            var activityLogs = FileHandlingService.ReadFile<ActivityLog>(FilePathResource.ActivityLogFilePath);
            activityLogs.Add(activityLog);
            FileHandlingService.WriteFile(activityLogs, FilePathResource.ActivityLogFilePath);
        }

        public void EditActivityLog(Guid activityLogId, ActivityLog activityLog)
        {
            var activityLogs = FileHandlingService.ReadFile<ActivityLog>(FilePathResource.ActivityLogFilePath);
            var activityLogToUpdate = activityLogs.FirstOrDefault(a => a.ActivityLogId == activityLogId);
            activityLogToUpdate.LogStatus = activityLog.LogStatus;
            activityLogToUpdate.EndTime = activityLog.EndTime;
            activityLogToUpdate.Duration = activityLog.Duration;
            FileHandlingService.WriteFile(activityLogs, FilePathResource.ActivityLogFilePath);
        }

        public IEnumerable<ActivityLog> FetchAllActivityLogs()
        {
            var activityLogs = FileHandlingService.ReadFile<ActivityLog>(FilePathResource.ActivityLogFilePath);
            return activityLogs.Select(a => new ActivityLog(a.ActivityLogId, a.StartTime, a.EndTime, a.Duration, a.LogStatus, a.TaskId)).ToList();
        }
    }
}
