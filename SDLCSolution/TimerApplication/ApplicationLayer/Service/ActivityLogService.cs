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
        IEnumerable<Activity> activities = this._activityService.GetAllActivitiesByUser(_currentSessionManager.CurrentSessionUser!.UserId);
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
        string? taskName = activityToStartTimer.ActivityName;
        string logName = $"{taskName}-Log{logs.Count() + 1}";
        this.CreateActivityLog(logName, activityToStartTimer.ActivityId);
        this._activityService.EditTaskStatus(activityToStartTimer.ActivityId, TimerStatus.Running);
        return new Result(true, "Timer started successfully!");
    }

    public void CreateActivityLog(string logName, Guid activityId)
    {
        ActivityLog activityLog = new ActivityLog(logName, DateTime.Now, null, TimeSpan.Zero, TimerStatus.Running, activityId, _currentSessionManager.CurrentSessionUser!.UserId);
        this._activityLogRepository.CreateActivityLog(activityLog);
    }

    private IEnumerable<ActivityLog> GetLogsByActivityId(Guid activityId)
    {
        return this.GetAllActivityLogs().Where(a => a.ActivityLogId == activityId);
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

    public IEnumerable<ActivityLog> GetAllActivityLogs()
    {
        return this._activityLogRepository.FetchAllActivityLogs().Where(log => log.UserId == _currentSessionManager.CurrentSessionUser?.UserId);
    }
}
