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
            List<Activity>? activities = FileHandlingService.ReadFile<List<Activity>>(FilePathResource.ActivityRepoFilePath);
            activities?.Add(activity);
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

        public IEnumerable<Activity> GetAllActivitiesByUser(Guid userId)
        {
            List<Activity> activities = FileHandlingService.ReadFile<List<Activity>>(FilePathResource.ActivityRepoFilePath) ?? [];
            return activities.Where(activity => activity.UserId == userId);
        }
    }
}
