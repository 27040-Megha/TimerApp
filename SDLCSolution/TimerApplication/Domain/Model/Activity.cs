using System;
using System.Collections.Generic;

namespace TimerApplication.Domain.Model
{
    public class Activity
    {
        public Activity(Guid activityId, string activityName, TimeSpan totalDuration, List<ActivityLog> activityLogs, Guid userId)
        {
            this.ActivityId = activityId;
            this.ActivityName = activityName;
            this.TotalDuration = totalDuration;
            this.ActivityLogs = activityLogs;
            this.UserId = userId;
        }
        public Guid ActivityId { get; set; }

        public string ActivityName { get; set; }

        public TimeSpan TotalDuration { get; set; }

        public List<ActivityLog> ActivityLogs { get; set; }

        public Guid UserId { get; set; }
    }
}
