using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Service;

public class ActivityService
{
    private readonly IActivityRepository _activityRepository;

    private readonly ISessionManager _currentSessionManager;

    public ActivityService(IActivityRepository activityRepository, ISessionManager currentSessionManager)
    {
        this._activityRepository = activityRepository;
        this._currentSessionManager = currentSessionManager;
    }

    public void CreateNewTask(Activity activity)
    {
        this._activityRepository.CreateActivity(activity);
    }

    public void EditTaskDuration(ActivityLog activityLog)
    {
        Activity activity = this.GetAllActivitiesByUser(activityLog.UserId)
            .FirstOrDefault(a => a.ActivityId == activityLog.ActivityId)
            ?? throw new KeyNotFoundException($"Activity with ID '{activityLog.ActivityId}' was not found.");
        activity.TotalDuration += activityLog.Duration;
        this._activityRepository.EditActivity(activityLog.ActivityId, activity);
    }

    public List<Activity> GetAllActivitiesByUser(Guid userId)
    {
        return this._activityRepository.GetAllActivitiesByUser(userId).ToList();
    }
}
