using System;
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
        return this.GetAllActivityLogs().Where(a => a.ActivityLogId == activityId).ToHashSet();
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

    public IEnumerable<ActivityLog> GetAllActivityLogs()
    {
        return this._activityLogRepository.FetchAllActivityLogs();
    }
}
