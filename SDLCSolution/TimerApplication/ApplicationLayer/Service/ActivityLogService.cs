using System;
using System.Collections.Generic;
using System.Linq;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Service
{
    public class ActivityLogService
    {
        private readonly IActivityLogRepository _activityLogRepository;

        private readonly ActivityService _activityService;

        public ActivityLogService(IActivityLogRepository activityLogRepository, ActivityService activityService)
        {
            this._activityLogRepository = activityLogRepository;
            this._activityService = activityService;
        }

        public void StartTimer(Guid taskId)
        {
            var logs = this.GetAllActivityLogs().Where(a => a.TaskId == taskId);
            string taskName = this._activityService.GetAllActivities().FirstOrDefault(a => a.ActivityId == taskId).ActivityName;
            var logName = $"{taskName}-Log{logs.Count() + 1}";
            var activityLog = new ActivityLog(Guid.NewGuid(), logName, DateTime.Now, null, TimeSpan.Zero, TimerStatus.Running, taskId, CurrentSessionManager.CurrentSessionUser.UserId);
            this._activityService.EditTaskStatus(taskId, TimerStatus.Running);
            this._activityLogRepository.CreateActivityLog(activityLog);
        }

        public void StopTimer(Guid activityLogId)
        {
            var activityLog = this._activityLogRepository.FetchAllActivityLogs().FirstOrDefault(a => a.ActivityLogId == activityLogId);
            activityLog.LogStatus = TimerStatus.Stopped;
            activityLog.EndTime = DateTime.Now;
            activityLog.Duration = (TimeSpan)(activityLog.EndTime - activityLog.StartTime);
            this._activityLogRepository.EditActivityLog(activityLogId, activityLog);
            this._activityService.EditTaskDuration(activityLog);
        }

        public List<ActivityLog> GetAllActivityLogs()
        {
            return this._activityLogRepository.FetchAllActivityLogs().ToList();
        }
    }
}
