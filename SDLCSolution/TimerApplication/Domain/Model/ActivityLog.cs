using System;
using TimerApplication.Domain.Enums;

namespace TimerApplication.Domain.Model;

public class ActivityLog
{
    public Guid ActivityLogId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public TimeSpan Duration { get; set; }

    public TimerStatus LogStatus { get; set; }

    public Guid ActivityId { get; set; }

    public Guid UserId { get; set; }
}
