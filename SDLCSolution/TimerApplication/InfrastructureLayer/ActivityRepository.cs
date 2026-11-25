using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer;

public class ActivityRepository : IActivityRepository
{
    public void CreateActivity(Activity activity)
    {
        List<Activity>? activities = FileHandlingService.ReadFile<List<Activity>>(FilePathResource.ActivityRepoFilePath) ?? new List<Activity>();
        activities?.Add(activity);
        FileHandlingService.WriteFile(activities, FilePathResource.ActivityRepoFilePath);
    }

    public void EditActivity(Guid activityIdToEdit, Activity activityToBeAssigned)
    {
        List<Activity> activities = FileHandlingService.ReadFile<List<Activity>>(FilePathResource.ActivityRepoFilePath) ?? new List<Activity>();
        Activity? activityToEdit = activities.FirstOrDefault(activity => activity.ActivityId.Equals(activityIdToEdit));
        if (activityToEdit == null)
        {
            return;
        }

        activityToEdit.ActivityName = activityToBeAssigned.ActivityName;
        activityToEdit.TotalDuration = activityToBeAssigned.TotalDuration;
        FileHandlingService.WriteFile(activities, FilePathResource.ActivityRepoFilePath);
    }

    public void DeleteActivity(Guid activityId)
    {
        List<Activity> activities = FileHandlingService.ReadFile<List<Activity>>(FilePathResource.ActivityRepoFilePath) ?? new List<Activity>();
        Activity? activityToDelete = activities.FirstOrDefault(a => a.ActivityId == activityId);
        activities.Remove(activityToDelete!);
        FileHandlingService.WriteFile(activities, FilePathResource.ActivityRepoFilePath);
    }
    public IEnumerable<Activity> GetAllActivitiesByUser(Guid userId)
    {
        List<Activity> activities = FileHandlingService.ReadFile<List<Activity>>(FilePathResource.ActivityRepoFilePath) ?? new List<Activity>();
        return activities.Where(activity => activity.UserId == userId);
    }

    public Activity? GetActivityById(Guid activityId)
    {
        List<Activity>? activities = FileHandlingService.ReadFile<List<Activity>>(FilePathResource.ActivityRepoFilePath) ?? new List<Activity>();
       if (activities == null)
       {
            return null;
       }

        return activities.FirstOrDefault(activity => activity.ActivityId == activityId);
    }
}
