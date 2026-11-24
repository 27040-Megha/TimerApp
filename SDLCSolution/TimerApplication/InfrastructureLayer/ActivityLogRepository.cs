using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        public void CreateActivityLog(ActivityLog activityLog)
        {
            List<ActivityLog>? activityLogs = FileHandlingService.ReadFile<List<ActivityLog>>(FilePathResource.ActivityLogFilePath);
            activityLogs?.Add(activityLog);
            FileHandlingService.WriteFile(activityLogs, FilePathResource.ActivityLogFilePath);
        }

        public void EditActivityLog(Guid activityLogId, ActivityLog activityLogToBeAssigned)
        {
            List<ActivityLog>? activityLogs = FileHandlingService.ReadFile<List<ActivityLog>>(FilePathResource.ActivityLogFilePath);
            ActivityLog? activityLogToUpdate = activityLogs!.FirstOrDefault(a => a.ActivityLogId == activityLogId);
            activityLogToUpdate!.LogStatus = activityLogToBeAssigned.LogStatus;
            activityLogToUpdate.EndTime = activityLogToBeAssigned.EndTime;
            activityLogToUpdate.Duration = activityLogToBeAssigned.Duration;
            FileHandlingService.WriteFile(activityLogs, FilePathResource.ActivityLogFilePath);
        }

        public IEnumerable<ActivityLog> FetchAllActivityLogs()
        {
            List<ActivityLog> activityLogs = FileHandlingService.ReadFile<List<ActivityLog>>(FilePathResource.ActivityLogFilePath) ?? [];
            return activityLogs;
        }
    }
}
