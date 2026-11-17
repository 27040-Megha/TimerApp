using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Service;

public class ActivityLogService
{
    private readonly IActivityLogRepository _activityLogRepository;

    private readonly ActivityService _activityService;

    public ActivityLogService(IActivityLogRepository activityLogRepository, ActivityService activityService)
    {
        this._activityLogRepository = activityLogRepository;
        this._activityService = activityService;
    }

    public void StartTimer(Guid activityId)
    {
        HashSet<ActivityLog> logs = this.GetLogsByActivityId(activityId);
        string taskName = this._activityService.GetAllActivities().FirstOrDefault(activity => activity.ActivityId == activityId).ActivityName;
        string logName = $"{taskName}-Log{logs.Count() + 1}";
        this.CreateActivityLog(logName, activityId);
        this._activityService.EditTaskStatus(activityId, TimerStatus.Running);
    }

    public void CreateActivityLog(string logName, Guid activityId)
    {
        ActivityLog activityLog = new ActivityLog(logName, DateTime.Now, null, TimeSpan.Zero, TimerStatus.Running, activityId, CurrentSessionManager.CurrentSessionUser.UserId);
        this._activityLogRepository.CreateActivityLog(activityLog);
    }

    private HashSet<ActivityLog> GetLogsByActivityId(Guid activityId)
    {
        return this.GetAllActivityLogs().Where(activity => activity.ActivityId == activityId).ToHashSet();
    }
    public void StopTimer(Guid activityLogId)
    {
        ActivityLog activityLog = this._activityLogRepository.FetchAllActivityLogs().FirstOrDefault(activityLog => activityLog.ActivityLogId == activityLogId);
        activityLog.LogStatus = TimerStatus.Stopped;
        activityLog.EndTime = DateTime.Now;
        activityLog.Duration = (TimeSpan)(activityLog.EndTime - activityLog.StartTime);
        this._activityLogRepository.EditActivityLog(activityLogId, activityLog);
        this._activityService.EditTaskDuration(activityLog);
    }

        public List<ActivityLog> GetAllActivityLogs()
        {
            return this._activityLogRepository.FetchAllActivityLogs().Where(a => a.UserId == CurrentSessionManager.CurrentSessionUser.UserId).ToList();
        }

        public void DeleteActivityLogsByTaskId(Guid activityId)
        {
            var activityLogs = this.GetAllActivityLogs().Where(a => a.ActivityId == activityId).ToList();
            foreach (var log in activityLogs)
            {
                this._activityLogRepository.DeleteActivityLog(log.ActivityLogId);
            }
        }

        public bool DeleteActivityLogByLogId(Guid activityLogId)
        {
            var activityLogToDelete = this.GetAllActivityLogs().FirstOrDefault(a => a.ActivityLogId == activityLogId);
            if (activityLogToDelete.LogStatus == TimerStatus.Running)
            {
                return false;
            }

            this._activityLogRepository.DeleteActivityLog(activityLogId);
            return true;
        }

        public void EditTaskLog(ActivityLog activityLogToBeEdited)
        {
            this._activityLogRepository.EditActivityLog(activityLogToBeEdited.ActivityLogId, activityLogToBeEdited);
        }

        public Result ExportLogs(int indexOfActivity)
        {
            var activities = this._activityService.GetAllActivities();
            if (indexOfActivity >= activities.Count)
            {
                return new Result(false, "Index Out Of Range");
            }

            var selectedActivity = activities[indexOfActivity];
            var logsToExport = this.GetAllActivityLogs().Where(log => log.TaskId == selectedActivity.ActivityId)
                .OrderBy(log => log.StartTime)
                .ToList();
            if (!logsToExport.Any())
            {
                return new Result(false, "No logs recorded for the activity to export");
            }

            var csvContent = this.GetCsvContent(logsToExport);
            this._activityLogRepository.ExportActivityLogsToCsv($"{selectedActivity.ActivityName}.csv", csvContent);
            return new Result(true, $"Logs exported Successfully");
        }

        private string GetCsvContent(List<ActivityLog> logsToExport)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Log Name, Status, Start Time, End Time, Duration");
            foreach (var log in logsToExport)
            {
                var startTime = log.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                var endTime = log.EndTime.HasValue ? log.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "In-Progress";
                var duration = log.Duration.ToString(@"hh\:mm\:ss");
                csvBuilder.AppendLine($"{log.LogName}, {log.LogStatus}, {startTime}, {endTime}, {duration}");
            }

            return csvBuilder.ToString();
        }
    }

