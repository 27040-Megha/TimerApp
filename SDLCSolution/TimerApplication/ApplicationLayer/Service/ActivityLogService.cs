using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public Result StartTimer(int index)
    {
        IEnumerable<Activity> activities = this._activityService.GetAllActivities();
        if (index >= activities.Count())
        {
            return new Result(false, "Index out of range");
        }

        Activity activityToStartTimer = activities.ElementAt(index);
        if (activityToStartTimer.TaskStatus == TimerStatus.Running)
        {
            return new Result(false, "Can't Start, Timer is already running for this task");
        }

        IEnumerable<ActivityLog> logs = this.GetLogsByActivityId(activityToStartTimer.ActivityId);
        string taskName = activityToStartTimer.ActivityName;
        string logName = $"{taskName}-Log{logs.Count() + 1}";
        this.CreateActivityLog(logName, activityToStartTimer.ActivityId);
        this._activityService.EditTaskStatus(activityToStartTimer.ActivityId, TimerStatus.Running);
        return new Result(true, "Timer started successfully!");
    }

    public void CreateActivityLog(string logName, Guid activityId)
    {
        ActivityLog activityLog = new ActivityLog(logName, DateTime.Now, null, TimeSpan.Zero, TimerStatus.Running, activityId, CurrentSessionManager.CurrentSessionUser.UserId);
        this._activityLogRepository.CreateActivityLog(activityLog);
    }

    private IEnumerable<ActivityLog> GetLogsByActivityId(Guid activityId)
    {
        return this.GetAllActivityLogs().Where(log => log.ActivityId == activityId);
    }

    public Result StopTimer(int index)
    {
        IEnumerable<ActivityLog> activeLogs = this.GetAllActiveTimerLogs();
        if (index >= activeLogs.Count())
        {
            return new Result(false, "Index out of range!");
        }

        ActivityLog logToStopTimer = activeLogs.ElementAt(index);
        logToStopTimer.LogStatus = TimerStatus.Stopped;
        logToStopTimer.EndTime = DateTime.Now;
        logToStopTimer.Duration = (TimeSpan)(logToStopTimer.EndTime - logToStopTimer.StartTime);
        this._activityLogRepository.EditActivityLog(logToStopTimer.ActivityLogId, logToStopTimer);
        this._activityService.EditTaskDuration(logToStopTimer);
        return new Result(true, "Timer stoppped successfully!");
    }

    public IEnumerable<ActivityLog> GetAllActiveTimerLogs()
    {
        return this.GetAllActivityLogs().Where(log => log.LogStatus == TimerStatus.Running);
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

    public bool EditTaskLog(ActivityLog activityLogToBeEdited)
    {
        bool isDuplicate = this.GetAllActivityLogs().Any(log =>
            log.ActivityLogId != activityLogToBeEdited.ActivityLogId &&
            log.ActivityId == activityLogToBeEdited.ActivityId &&
            string.Equals(log.LogName, activityLogToBeEdited.LogName, StringComparison.OrdinalIgnoreCase));
        if (isDuplicate)
        {
            return false;
        }

        this._activityLogRepository.EditActivityLog(activityLogToBeEdited.ActivityLogId, activityLogToBeEdited);
        return true;
    }

    public IEnumerable<ActivityLog> GetAllActivityLogs()
    {
        return this._activityLogRepository.FetchAllActivityLogs().Where(a => a.UserId == CurrentSessionManager.CurrentSessionUser.UserId);
    }

    public Result GetAllActivityLogsByActivityIndex(int index, out IEnumerable<ActivityLog> activityLogs)
    {
        var activities = this._activityService.GetAllActivities();
        activityLogs = new List<ActivityLog>();
        if (index >= activities.Count())
        {
            return new Result(false, "Index out of range");
        }

        Guid activityId = activities.ElementAt(index).ActivityId;
        activityLogs = this.GetLogsByActivityId(activityId).OrderBy(a => a.StartTime);
        if (activityLogs.Count() == 0)
        {
            return new Result(false, "No activity logs recorded for the activity!");
        }

        return new Result(true, "Logs fetched successfull!");
    }

    public Result ExportLogs(int indexOfActivity)
    {
        var activities = this._activityService.GetAllActivities();
        if (indexOfActivity >= activities.Count)
        {
            return new Result(false, "Index Out Of Range");
        }

        var selectedActivity = activities[indexOfActivity];
        var logsToExport = this.GetAllActivityLogs().Where(log => log.ActivityId == selectedActivity.ActivityId)
             .OrderBy(log => log.StartTime)
             .ToList();
        if (!logsToExport.Any())
        {
            return new Result(false, "No logs recorded for the activity to export");
        }

        this._activityLogRepository.ExportActivityLogsToCsv($"{selectedActivity.ActivityName}-{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.csv", logsToExport);
        return new Result(true, $"Logs exported Successfully");
    }

    public Result ImportLogs(string filePath, int index)
    {
        try
        {
            var activities = this._activityService.GetAllActivities();
            if (index < 0 || index >= activities.Count)
            {
                return new Result(false, "Index Out Of Range");
            }

            var activitySelected = activities[index];
            var importedLogs = this._activityLogRepository.ImportActivityLogsFromCsv(filePath, activitySelected.ActivityId);
            return ProcessAndSaveLogs(activitySelected, importedLogs);
        }
        catch (Exception ex) when (ex is FormatException || ex is FileNotFoundException)
        {
            return new Result(false, ex.Message);
        }
    }

    private Result ProcessAndSaveLogs(Activity activity, List<ActivityLog> importedLogs)
    {
        var existingLogs = this.GetAllActivityLogs()
            .Where(a => a.ActivityId == activity.ActivityId)
            .ToList();

        TimeSpan totalDuration = TimeSpan.Zero;
        int importedCount = 0;
        var skippedEntries = new List<ActivityLog>();
        foreach (var log in importedLogs)
        {
            if (!IsValidAndNonOverlapping(log, existingLogs))
            {
                skippedEntries.Add(log);
                continue;
            }

            this._activityLogRepository.CreateActivityLog(log);
            existingLogs.Add(log);
            totalDuration += log.Duration;
            importedCount++;
        }

        if (importedCount == 0)
        {
            return new Result(false, "No logs were imported because time overlaps with existing activity logs or timestamps are invalid.");
        }

        this._activityService.UpdateTaskTotalDuration(activity, totalDuration);
        if (!skippedEntries.Any())
        {
            return new Result(true, "Logs imported successfully!");
        }

        string skippedDetails = string.Join("\n", skippedEntries.Select(log =>
            $"- {log.LogName} ({log.StartTime:yyyy-MM-dd HH:mm:ss} to {log.EndTime:yyyy-MM-dd HH:mm:ss})"));

        return new Result(true, $"{importedCount} logs imported. Overlapping or invalid log entries were skipped:\n{skippedDetails}");
    }

    private static bool IsValidAndNonOverlapping(ActivityLog log, List<ActivityLog> existingLogs)
    {
        if (!log.EndTime.HasValue || log.EndTime <= log.StartTime)
        {
            return false;
        }

        return !existingLogs.Any(existing =>
            existing.EndTime.HasValue &&
            log.StartTime < existing.EndTime &&
            log.EndTime > existing.StartTime);
    }
}

