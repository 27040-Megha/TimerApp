using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Interface;

public interface IActivityRepository
{
    public void CreateActivity(Activity activity);

    public void DeleteActivity(Guid activityId);

    public void EditActivity(Guid activityId, Activity activity);

    public IEnumerable<Activity> GetAllActivitiesByUser(Guid userId);

    public Activity? GetActivityById(Guid activityId);
}
