using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Enums;
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

    public void AddToTaskDuration(Guid activityId, TimeSpan durationToAdd)
    {
        if (durationToAdd < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(durationToAdd), "Duration to add cannot be negative.");
        }
    
        Activity? activity = this._activityRepository.GetActivityById(activityId);
        if (activity == null)
        {
            throw new KeyNotFoundException($"Activity with ID '{activityId}' was not found.");
        }

        activity.TotalDuration += durationToAdd;
        this._activityRepository.EditActivity(activityId, activity);
    }


    public void RemoveLogDuration(Guid taskId, TimeSpan duration)
    {
        Activity? activity = this.GetAllActivitiesByUser(_currentSessionManager.CurrentSessionUser!.UserId).FirstOrDefault(a => a.ActivityId == taskId);
        if (activity == null)
        {
            return;
        }

        activity.TotalDuration -= duration;
        this._activityRepository.EditActivity(activity.ActivityId, activity);
    }

    public Result EditTaskName(int index, string input)
    {
        List<Activity> activities = this.GetAllActivitiesByUser(_currentSessionManager.CurrentSessionUser!.UserId);
        if (activities.Count == 0)
        {
            return new Result(false, "No activities found to edit");
        }

        if (index >= activities.Count)
        {
            return new Result(false, "Index out of range");
        }

        Activity activityToEdit = activities.ElementAt(index);
        activityToEdit.ActivityName = input;
        this._activityRepository.EditActivity(activityToEdit.ActivityId, activityToEdit);
        return new Result(true, "Edit Successful!");
    }

    public List<Activity> GetAllActivitiesByUser(Guid userId)
    {
        return this._activityRepository.GetAllActivitiesByUser(userId).ToList();
    }

    public void DeleteActivityById(Guid activityId)
    {
        this._activityRepository.DeleteActivity(activityId);
    }
    public void UpdateTaskTotalDuration(Activity activitySelected, TimeSpan totalTaskDuration)
    {
        Activity? activity = this.GetAllActivitiesByUser(_currentSessionManager.CurrentSessionUser!.UserId).FirstOrDefault(a => a.ActivityId == activitySelected.ActivityId);
        activity!.TotalDuration += totalTaskDuration;
        this._activityRepository.EditActivity(activity.ActivityId, activity);
    }
}
