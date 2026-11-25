using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;
using TimerApplication.InfrastructureLayer;

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

    public Result StartTimer(Guid activityIdToStartTimer)
    {
        List<Activity> activities = this._activityService.GetAllActivitiesByUser(_currentSessionManager.CurrentSessionUser!.UserId);
        Activity activityToStartTimer = activities.FirstOrDefault(activity => activity.ActivityId == activityIdToStartTimer)!;
        List<ActivityLog> logs = this.GetLogsByActivityId(activityToStartTimer.ActivityId).ToList();
        if (logs.Any(log => log.LogStatus == TimerStatus.Running))
        {
            return new Result(false, "Can't Start, Timer is already running for this task");
        }

        string logName = $"{activityToStartTimer.ActivityName}-{DateTime.Now:yyyyMMdd-HHmmss}";
        this.CreateActivityLog(logName, activityToStartTimer.ActivityId);
        return new Result(true, "Timer started successfully!");
    }


    private IEnumerable<ActivityLog> GetLogsByActivityId(Guid activityId)
    {
        return this.GetAllActivityLogs().Where(log => log.ActivityId == activityId);
    }

    public Result StopTimer(Guid logIdToStopTimer)
    {
        List<ActivityLog> activeLogs = this.GetAllActiveTimerLogs().ToList();
        ActivityLog logToStopTimer = activeLogs.FirstOrDefault(log => log.ActivityLogId == logIdToStopTimer)!;
        DateTime stoppedAt = DateTime.UtcNow;
        logToStopTimer.LogStatus = TimerStatus.Stopped;
        logToStopTimer.EndTime = stoppedAt;
        logToStopTimer.Duration = stoppedAt - logToStopTimer.StartTime;
        this._activityLogRepository.EditActivityLog(logToStopTimer.ActivityLogId, logToStopTimer);
        this._activityService.AddToTaskDuration(logToStopTimer.ActivityId, logToStopTimer.Duration);
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
        this._activityLogRepository.DeleteActivityLogsByActivityId(activityId);
    }

    public Result DeleteActivityLogByLogId(Guid logIdToDelete)
    {
        List<ActivityLog> activityLogs = this.GetAllActivityLogs().ToList();
        ActivityLog activityLogToDelete = activityLogs.FirstOrDefault(log => log.ActivityLogId == logIdToDelete)!;
        if (activityLogToDelete.LogStatus == TimerStatus.Running)
        {
            return new Result(false, "Cannot delete a log while running timer! Stop the timer first");
        }

        this._activityLogRepository.DeleteActivityLog(activityLogToDelete.ActivityLogId);
        this._activityService.RemoveLogDuration(activityLogToDelete.ActivityId, activityLogToDelete.Duration);
        return new Result(true, "Activity Log Deleted Successfully!");
    }


    public Result EditTaskLog(Guid logIdToEdit, string? input)
    {
        List<ActivityLog> activityLogs = this.GetAllActivityLogs().ToList();
        ActivityLog activityLogToBeEdited = activityLogs.FirstOrDefault(log => log.ActivityLogId == logIdToEdit)!;
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

    public Result<ActivityLog> GetAllActivityLogsByActivityIndex(Guid activityId)
    {
        List<ActivityLog> activityLogs = this.GetLogsByActivityId(activityId)
            .OrderBy(a => a.StartTime)
            .ToList();

        if (!activityLogs.Any())
        {
            return new Result<ActivityLog>(false, "No activity logs recorded for the activity!");
        }

        return new Result<ActivityLog>(true, "Logs fetched successfully!", activityLogs);
    }

    public Result DeleteActivity(Guid activityId)
    {
        List<Activity> activities = this._activityService.GetAllActivitiesByUser(_currentSessionManager.CurrentSessionUser!.UserId);
        Activity activityToDelete = activities.FirstOrDefault(activity => activity.ActivityId == activityId)!;
        bool isTimerRunning = this.GetLogsByActivityId(activityToDelete.ActivityId)
            .Any(log => log.LogStatus == TimerStatus.Running);

        if (isTimerRunning)
        {
            return new Result(false, ErrorCode.TimerRunning);
        }

        this._activityService.DeleteActivityById(activityToDelete.ActivityId);
        this.DeleteActivityLogsByTaskId(activityToDelete.ActivityId);
        return new Result(true, ErrorCode.None);
    }

    public Result ImportLogs(string filePath, Guid activityId)
    {
        try
        {
            Guid userId = _currentSessionManager.CurrentSessionUser!.UserId;
            List<Activity> activities = this._activityService.GetAllActivitiesByUser(userId);
            Activity activitySelected = activities.FirstOrDefault(activity => activity.ActivityId == activityId)!;
            string csvContent = FileHandlingService.ReadTextFile(filePath);
            List<ActivityLog> importedLogs = CsvService.DeserializeLogs(csvContent, activitySelected.ActivityId, userId);

            return ProcessAndSaveLogs(activitySelected, importedLogs);
        }
        catch (Exception ex) when (ex is FormatException or FileNotFoundException or IOException or UnauthorizedAccessException or ArgumentException)
        {
            return new Result(false, ex.Message);
        }
    }

    private Result ProcessAndSaveLogs(Activity activity, List<ActivityLog> importedLogs)
    {
        List<ActivityLog> existingLogs = this.GetAllActivityLogs()
            .Where(a => a.ActivityId == activity.ActivityId)
            .ToList();
        TimeSpan totalDuration = TimeSpan.Zero;
        int importedCount = 0;
        List<ActivityLog> skippedEntries = new List<ActivityLog>();
        foreach (ActivityLog log in importedLogs)
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

    public TimeSpan GetCurrentActivityDuration(Activity activity, IEnumerable<ActivityLog> activeLogs)
    {
        TimeSpan activeElapsed = activeLogs
            .Where(log => log.ActivityId == activity.ActivityId)
            .Aggregate(TimeSpan.Zero, (sum, log) => sum + (DateTime.UtcNow - log.StartTime));

        return activity.TotalDuration + activeElapsed;
    }

    public Result ExportLogs(Guid activityId)
    {
        try
        {
            Guid userId = _currentSessionManager.CurrentSessionUser!.UserId;
            List<Activity> activities = this._activityService.GetAllActivitiesByUser(userId);
            Activity selectedActivity = activities.FirstOrDefault(activity => activity.ActivityId == activityId)!;
            List<ActivityLog> logsToExport = this.GetAllActivityLogs()
                .Where(log => log.ActivityId == selectedActivity.ActivityId &&
                              log.LogStatus == TimerStatus.Stopped &&
                              log.EndTime.HasValue)
                .OrderBy(log => log.StartTime)
                .ToList();

            if (!logsToExport.Any())
            {
                return new Result(false, "No completed logs recorded for the activity to export");
            }

            string csvContent = CsvService.SerializeLogs(logsToExport);
            string fileName = $"{selectedActivity.ActivityName}-{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.csv";
            string savedPath = FileHandlingService.WriteCsvToDownloads(fileName, csvContent);
            return new Result(true, $"Logs exported successfully to {savedPath}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            return new Result(false, ex.Message);
        }
    }
}

