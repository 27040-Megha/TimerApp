using System;
using TimerApplication.Domain.Enums;

namespace TimerApplication.Domain.Model
{
    public class ActivityLog
    {
        public ActivityLog(Guid activitylogId, DateTime startTime, DateTime? endTime, TimeSpan duration, TimerStatus logStatus, Guid taskId)
        {
            this.ActivityLogId = activitylogId;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Duration = duration;
            this.LogStatus = logStatus;
            this.TaskId = taskId;
        }

        public Guid ActivityLogId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public TimeSpan Duration { get; set; }

    public TimerStatus LogStatus { get; set; }

    public Guid ActivityId { get; set; }

    public Guid UserId { get; set; }
}
