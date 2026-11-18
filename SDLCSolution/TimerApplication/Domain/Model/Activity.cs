using System;

namespace TimerApplication.Domain.Model;

public class Activity
{
    public Activity()
    {

    }

    public Activity(Guid activityId, string activityName, TimeSpan totalDuration, Guid userId)
    {
        this.ActivityId = activityId;
        this.ActivityName = activityName;
        this.TotalDuration = totalDuration;
        this.UserId = userId;
    }

    public Activity(string activityName, TimeSpan totalDuration, Guid userId)
    {
        this.ActivityId = Guid.NewGuid();
        this.ActivityName = activityName;
        this.TotalDuration = totalDuration;
        this.UserId = userId;
    }
    public Guid ActivityId { get; set; }

    public string ActivityName { get; set; }

    public TimeSpan TotalDuration { get; set; }

    public Guid UserId { get; set; }
}
