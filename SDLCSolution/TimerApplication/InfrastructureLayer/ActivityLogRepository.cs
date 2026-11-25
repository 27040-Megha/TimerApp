using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer;

public class ActivityLogRepository : IActivityLogRepository
{
    public void CreateActivityLog(ActivityLog activityLog)
    {
        List<ActivityLog>? activityLogs = FileHandlingService.ReadFile<List<ActivityLog>>(FilePathResource.ActivityLogFilePath) ?? new List<ActivityLog>();
        activityLogs?.Add(activityLog);
        FileHandlingService.WriteFile(activityLogs, FilePathResource.ActivityLogFilePath);
    }

    public void DeleteActivityLog(Guid activityLogId)
    {
        List<ActivityLog> activityLogs = FileHandlingService.ReadFile<List<ActivityLog>>(FilePathResource.ActivityLogFilePath) ?? new List<ActivityLog>();
        ActivityLog? logToDelete = activityLogs.FirstOrDefault(log => log.ActivityLogId == activityLogId);
        activityLogs.Remove(logToDelete!);
        FileHandlingService.WriteFile(activityLogs, FilePathResource.ActivityLogFilePath);
    }

    public void DeleteActivityLogsByActivityId(Guid activityId)
    {
        List<ActivityLog> activityLogs = FileHandlingService.ReadFile<List<ActivityLog>>(FilePathResource.ActivityLogFilePath) ?? new List<ActivityLog>();
        int removedCount = activityLogs.RemoveAll(log => log.ActivityId == activityId);
        if (removedCount > 0)
        {
            FileHandlingService.WriteFile(activityLogs, FilePathResource.ActivityLogFilePath);
        }
    }

    public void EditActivityLog(Guid activityLogId, ActivityLog activityLogToBeAssigned)
    {
        List<ActivityLog> activityLogs = FileHandlingService.ReadFile<List<ActivityLog>>(FilePathResource.ActivityLogFilePath) ?? new List<ActivityLog>();
        ActivityLog? activityLogToUpdate = activityLogs.FirstOrDefault(a => a.ActivityLogId == activityLogId);
        if (activityLogToUpdate == null)
        {
            return;
        }

        activityLogToUpdate.LogName = activityLogToBeAssigned.LogName;
        activityLogToUpdate.LogStatus = activityLogToBeAssigned.LogStatus;
        activityLogToUpdate.EndTime = activityLogToBeAssigned.EndTime;
        activityLogToUpdate.Duration = activityLogToBeAssigned.Duration;
        FileHandlingService.WriteFile(activityLogs, FilePathResource.ActivityLogFilePath);
    }

    public void ExportActivityLogsToCsv(string fileName, List<ActivityLog> logsToExport)
    {
        FileHandlingService.ExportToCSV(fileName, logsToExport);
    }

    public List<ActivityLog> ImportActivityLogsFromCsv(string filePath, Guid taskId, Guid userId)
    {
        return FileHandlingService.DeserializeActivityLogsFromCsv(filePath, taskId, userId);
    }

    public IEnumerable<ActivityLog> FetchAllActivityLogs()
    {
        List<ActivityLog> activityLogs = FileHandlingService.ReadFile<List<ActivityLog>>(FilePathResource.ActivityLogFilePath) ?? new List<ActivityLog>();
        return activityLogs;
    }
}
