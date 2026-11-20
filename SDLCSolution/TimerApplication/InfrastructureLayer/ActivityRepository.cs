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

        public void EditActivity(Guid activityIdToEdit, Activity activityToBeAssigned)
        {
            var activities = FileHandlingService.ReadFile<HashSet<Activity>>(FilePathResource.ActivityRepoFilePath);
            var activityToEdit = activities.FirstOrDefault(activity => activity.ActivityId.Equals(activityIdToEdit));
            activityToEdit.ActivityName = activityToBeAssigned.ActivityName;
            activityToEdit.TaskStatus = activityToBeAssigned.TaskStatus;
            activityToEdit.TotalDuration = activityToBeAssigned.TotalDuration;
            FileHandlingService.WriteFile(activities, FilePathResource.ActivityRepoFilePath);
        }

        public Activity GetActivityById(Guid activityId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Activity> GetAllActivities()
        {
            return FileHandlingService.ReadFile<HashSet<Activity>>(FilePathResource.ActivityRepoFilePath);            
        }
    }
}
