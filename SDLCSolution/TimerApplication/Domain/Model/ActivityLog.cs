using System;
using TimerApplication.Domain.Enums;

namespace TimerApplication.Domain.Model;

public class ActivityLog
{
    public ActivityLog()
    {
    }

    public ActivityLog(Guid activitylogId, string logName, DateTime startTime, DateTime? endTime, TimeSpan duration, TimerStatus logStatus, Guid taskId, Guid userId)
    {
        this.ActivityLogId = activitylogId;
        this.LogName = logName;
        this.StartTime = startTime;
        this.EndTime = endTime;
        this.Duration = duration;
        this.LogStatus = logStatus;
        this.ActivityId = taskId;
        this.UserId = userId;
    }

    public ActivityLog(string logName, DateTime startTime, DateTime? endTime, TimeSpan duration, TimerStatus logStatus, Guid taskId, Guid userId)
    {
        this.ActivityLogId = Guid.NewGuid();
        this.LogName = logName;
        this.StartTime = startTime;
        this.EndTime = endTime;
        this.Duration = duration;
        this.LogStatus = logStatus;
        this.ActivityId = taskId;
        this.UserId = userId;
    }

    public Guid ActivityLogId { get; set; }

    public string? LogName { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public TimeSpan Duration { get; set; }

    public TimerStatus LogStatus { get; set; }

    public Guid ActivityId { get; set; }

    public Guid UserId { get; set; }
}