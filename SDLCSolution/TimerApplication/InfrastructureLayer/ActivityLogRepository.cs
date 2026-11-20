using System;
using System.Collections.Generic;
using System.Linq;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        public void CreateActivityLog(ActivityLog activityLog)
        {
            var activityLogs = FileHandlingService.ReadFile<HashSet<ActivityLog>>(FilePathResource.ActivityLogFilePath);
            activityLogs.Add(activityLog);
            FileHandlingService.WriteFile(activityLogs, FilePathResource.ActivityLogFilePath);
        }

        public void DeleteActivityLog(Guid activityLogId)
        {
            var activityLogs = FileHandlingService.ReadFile<HashSet<ActivityLog>>(FilePathResource.ActivityLogFilePath);
            var logToDelete = activityLogs.FirstOrDefault(a => a.ActivityLogId == activityLogId);
            activityLogs.Remove(logToDelete);
            FileHandlingService.WriteFile(activityLogs, FilePathResource.ActivityLogFilePath);
        }

        public void EditActivityLog(Guid activityLogId, ActivityLog activityLog)
        {
            var activityLogs = FileHandlingService.ReadFile<HashSet<ActivityLog>>(FilePathResource.ActivityLogFilePath);
            var activityLogToUpdate = activityLogs.FirstOrDefault(a => a.ActivityLogId == activityLogId);
            activityLogToUpdate.LogName = activityLog.LogName;
            activityLogToUpdate.LogStatus = activityLog.LogStatus;
            activityLogToUpdate.EndTime = activityLog.EndTime;
            activityLogToUpdate.Duration = activityLog.Duration;
            FileHandlingService.WriteFile(activityLogs, FilePathResource.ActivityLogFilePath);
        }

        public void ExportActivityLogsToCsv(string fileName, List<ActivityLog> logsToExport)
        {
            FileHandlingService.ExportToCSV(fileName, logsToExport);
        }

        public List<ActivityLog> ImportActivityLogsFromCsv(string filePath, Guid taskId)
        {
            return FileHandlingService.DeserializeActivityLogsFromCsv(filePath, taskId);
        }

        public IEnumerable<ActivityLog> FetchAllActivityLogs()
        {
            return FileHandlingService.ReadFile<HashSet<ActivityLog>>(FilePathResource.ActivityLogFilePath);          
        }

        public List<ActivityLog> ImportActivityLogsFromCsv(string filePath, Guid activityId)
        {
            throw new NotImplementedException();
        }
    }
}
