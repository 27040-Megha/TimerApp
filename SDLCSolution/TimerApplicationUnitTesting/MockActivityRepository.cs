using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplicationUnitTesting;

public class MockActivityRepository : IActivityRepository
{
    private readonly List<Activity> _activities;

    public MockActivityRepository()
    {
        _activities = new List<Activity>();
    }

    public void CreateActivity(Activity activity)
    {
        _activities.Add(activity);
    }

    public void DeleteActivity(Guid activityId)
    {
        Activity? activityToDelete = GetActivityById(activityId);
        if (activityToDelete != null)
        {
            _activities.Remove(activityToDelete);
        }
    }

    public void EditActivity(Guid activityIdToEdit, Activity activity)
    {
        throw new NotImplementedException();
    }

    public Activity? GetActivityById(Guid activityId)
    {
        return GetAllActivities().FirstOrDefault(activity => activity.ActivityId == activityId);
    }

    public IEnumerable<Activity> GetAllActivities()
    {
        return _activities;
    }
}
