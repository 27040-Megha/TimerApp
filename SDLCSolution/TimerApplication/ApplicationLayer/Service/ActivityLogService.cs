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

    public TimeSpan GetCurrentActivityDuration(Activity activity, IEnumerable<ActivityLog> activeLogs)
    {
        TimeSpan activeElapsed = activeLogs
            .Where(log => log.ActivityId == activity.ActivityId)
            .Aggregate(TimeSpan.Zero, (sum, log) => sum + (DateTime.UtcNow - log.StartTime));

        return activity.TotalDuration + activeElapsed;
    }

}

