using System;
using System.Collections.Generic;
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

    private IEnumerable<ActivityLog> GetLogsByActivityId(Guid activityId)
    {
        return this.GetAllActivityLogs().Where(activity => activity.ActivityId == activityId);
    }

    public IEnumerable<ActivityLog> GetAllActiveTimerLogs()
    {
        return this.GetAllActivityLogs().Where(log => log.LogStatus == TimerStatus.Running);
    }

    public IEnumerable<ActivityLog> GetAllActivityLogs()
    {
        return this._activityLogRepository.FetchAllActivityLogs().Where(log => log.UserId == _currentSessionManager.CurrentSessionUser?.UserId);
    }

    public Result GetAllActivityLogsByActivityIndex(int index, out IEnumerable<ActivityLog> activityLogs)
    {
        List<Activity> activities = this._activityService.GetAllActivities();
        activityLogs = new List<ActivityLog>();
        if (index >= activities.Count())
        {
            return new Result(false, "Index out of range");
        }

        Guid activityId = activities.ElementAt(index).ActivityId;
        activityLogs = this.GetLogsByActivityId(activityId).OrderBy(a => a.StartTime);
        if (!activityLogs.Any())
        {
            return new Result(false, "No activity logs recorded for the activity!");
        }

        return new Result(true, "Logs fetched successfully!");
    }
}
