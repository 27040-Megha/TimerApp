using System;
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

        public void StartTimer(Guid taskId)
        {
            var activityLog = new ActivityLog(Guid.NewGuid(), DateTime.Now, null, TimeSpan.Zero, TimerStatus.Running, taskId);
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
    }
}
