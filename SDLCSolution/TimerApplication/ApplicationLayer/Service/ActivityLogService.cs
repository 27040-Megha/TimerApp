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

    private readonly ISessionManager _currentSessionManager;

    public ActivityLogService(IActivityLogRepository activityLogRepository, ActivityService activityService, ISessionManager currentSessionManager)
    {
        this._activityLogRepository = activityLogRepository;
        this._activityService = activityService;
        this._currentSessionManager = currentSessionManager;
    }

    public Result StartTimer(int index)
    {
        List<Activity> activities = this._activityService.GetAllActivitiesByUser(_currentSessionManager.CurrentSessionUser!.UserId);
        if (index >= activities.Count())
        {
            return new Result(false, "Index out of range");
        }

        Activity activityToStartTimer = activities[index];
        List<ActivityLog> logs = this.GetLogsByActivityId(activityToStartTimer.ActivityId).ToList();
        if (logs.Any(log => log.LogStatus == TimerStatus.Running))
        {
            return new Result(false, "Can't Start, Timer is already running for this task");
        }

        string logName = $"{activityToStartTimer.ActivityName}-Log{logs.Count + 1}";
        this.CreateActivityLog(logName, activityToStartTimer.ActivityId);
        return new Result(true, "Timer started successfully!");
    }

    private IEnumerable<ActivityLog> GetLogsByActivityId(Guid activityId)
    {
        return this.GetAllActivityLogs().Where(log => log.ActivityId == activityId);
    }

    public Result StopTimer(int index)
    {
        List<ActivityLog> activeLogs = this.GetAllActiveTimerLogs().ToList();
        if (index >= activeLogs.Count)
        {
            return new Result(false, "Index out of range!");
        }

        ActivityLog logToStopTimer = activeLogs[index];
        DateTime stoppedAt = DateTime.UtcNow;
        logToStopTimer.LogStatus = TimerStatus.Stopped;
        logToStopTimer.EndTime = stoppedAt;
        logToStopTimer.Duration = stoppedAt - logToStopTimer.StartTime;
        this._activityLogRepository.EditActivityLog(logToStopTimer.ActivityLogId, logToStopTimer);
        this._activityService.EditTaskDuration(logToStopTimer);
        return new Result(true, "Timer stopped successfully!");
    }

    public void CreateActivityLog(string logName, Guid activityId)
    {
        ActivityLog activityLog = new ActivityLog(logName, DateTime.UtcNow, null, TimeSpan.Zero, TimerStatus.Running, activityId, _currentSessionManager.CurrentSessionUser!.UserId);
        this._activityLogRepository.CreateActivityLog(activityLog);
    }

    public IEnumerable<ActivityLog> GetAllActiveTimerLogs()
    {
        return this.GetAllActivityLogs().Where(log => log.LogStatus == TimerStatus.Running);
    }

    public void DeleteActivityLogsByTaskId(Guid activityId)
    {
        var activityLogs = this.GetAllActivityLogs().Where(a => a.ActivityId == activityId).ToList();
        foreach (var log in activityLogs)
        {
            this._activityLogRepository.DeleteActivityLog(log.ActivityLogId);
        }
    }

    public Result DeleteActivityLogByLogId(int index)
    {
        var activityLogs = this.GetAllActivityLogs().ToList();
        if (activityLogs.Count == 0)
        {
            return new Result(false, "No activity logs found to delete");
        }

        if (index >= activityLogs.Count)
        {
            return new Result(false, "Index out of range");
        }

        var activityLogToDelete = activityLogs.ElementAt(index);
        if (activityLogToDelete.LogStatus == TimerStatus.Running)
        {
            return new Result(false, "Cannot delete a log while running timer! Stop the timer first");
        }

        this._activityLogRepository.DeleteActivityLog(activityLogToDelete.ActivityLogId);
        this._activityService.RemoveLogDuration(activityLogToDelete.ActivityId, activityLogToDelete.Duration);
        return new Result(true, "Activity Log Deleted Successfully!");
    }


    public Result EditTaskLog(int index, string input)
    {
        var activityLogs = this.GetAllActivityLogs().ToList();
        if (activityLogs.Count == 0)
        {
            return new Result(false, "No activity logs found to edit");
        }

        if (index >= activityLogs.Count)
        {
            return new Result(false, "Index out of range");
        }

        var activityLogToBeEdited = activityLogs.ElementAt(index);
        bool isDuplicate = activityLogs.Any(log =>
            log.ActivityLogId != activityLogToBeEdited.ActivityLogId &&
            log.ActivityId == activityLogToBeEdited.ActivityId &&
            string.Equals(log.LogName, input, StringComparison.OrdinalIgnoreCase));

        if (isDuplicate)
        {
            return new Result(false, "A log with this name already exists for this task!");
        }

        activityLogToBeEdited.LogName = input;
        this._activityLogRepository.EditActivityLog(activityLogToBeEdited.ActivityLogId, activityLogToBeEdited);
        return new Result(true, "Edit Successful!");
    }

    public IEnumerable<ActivityLog> GetAllActivityLogs()
    {
        return this._activityLogRepository.FetchAllActivityLogs().Where(log => log.UserId == _currentSessionManager.CurrentSessionUser?.UserId);
    }

    public Result<ActivityLog> GetAllActivityLogsByActivityIndex(int index)
    {
        List<Activity>? activities = this._activityService.GetAllActivitiesByUser(_currentSessionManager.CurrentSessionUser!.UserId);
        if (index < 0 || index >= activities.Count)
        {
            return new Result<ActivityLog>(false, "Index out of range");
        }

        Guid activityId = activities[index].ActivityId;
        List<ActivityLog> activityLogs = this.GetLogsByActivityId(activityId)
            .OrderBy(a => a.StartTime)
            .ToList();

        if (!activityLogs.Any())
        {
            return new Result<ActivityLog>(false, "No activity logs recorded for the activity!");
        }

        return new Result<ActivityLog>(true, "Logs fetched successfully!", activityLogs);
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

