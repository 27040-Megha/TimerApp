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
    }

