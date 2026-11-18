using System;
using System.Collections.Generic;
using System.Linq;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer
{
    public class ActivityRepository : IActivityRepository
    {
        public void CreateActivity(Activity activity)
        {
            var activities = FileHandlingService.ReadFile<HashSet<Activity>>(FilePathResource.ActivityRepoFilePath);
            activities.Add(activity);
            FileHandlingService.WriteFile(activities, FilePathResource.ActivityRepoFilePath);
        }

        public void DeleteActivity(Guid ActivityId)
        {
            throw new NotImplementedException();
        }

        public void EditActivity(Guid activityIdToEdit, string TaskName)
        {
            throw new NotImplementedException();
        }

        public Activity GetActivityById(Guid activityId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Activity> GetAllActivities()
        {
            var activities = FileHandlingService.ReadFile<HashSet<Activity>>(FilePathResource.ActivityRepoFilePath);
            return activities.Select(activity => new Activity(activity.ActivityId, activity.ActivityName, activity.TotalDuration, activity.UserId));
        }
    }
}
