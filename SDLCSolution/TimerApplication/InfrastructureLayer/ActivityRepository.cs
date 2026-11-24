using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.InfrastructureLayer;

public class ActivityRepository : IActivityRepository
{
    public void CreateActivity(Activity activity)
    {
        List<Activity>? activities = FileHandlingService.ReadFile<List<Activity>>(FilePathResource.ActivityRepoFilePath);
        activities?.Add(activity);
        FileHandlingService.WriteFile(activities, FilePathResource.ActivityRepoFilePath);
    }

    public IEnumerable<Activity> GetAllActivitiesByUser(Guid userId)
    {
        List<Activity> activities = FileHandlingService.ReadFile<List<Activity>>(FilePathResource.ActivityRepoFilePath) ?? [];
        return activities.Where(activity => activity.UserId == userId);
    }
}
