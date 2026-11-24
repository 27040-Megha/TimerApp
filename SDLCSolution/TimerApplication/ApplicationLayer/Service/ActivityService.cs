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


    public Result DeleteActivity(int index)
    {
        var activities = this.GetAllActivities();
        if (activities.Count == 0)
        {
            return new Result(false, ErrorCode.NoActivitiesFound);
        }

        if (index >= activities.Count)
        {
            return new Result(false, ErrorCode.IndexOutOfRange);
        }
        var activityToDelete = activities.ElementAt(index);
        if (activityToDelete.TaskStatus == TimerStatus.Running)
        {
            return new Result(false, ErrorCode.TimerRunning);
        }

        this._activityRepository.DeleteActivity(activityToDelete.ActivityId);
        return new Result(true, ErrorCode.None);
    }

    public void RemoveLogDuration(Guid taskId, TimeSpan duration)
    {
        var activity = this.GetAllActivities().FirstOrDefault(a => a.ActivityId == taskId);
        activity.TotalDuration -= duration;
        this._activityRepository.EditActivity(activity.ActivityId, activity);
    }

    public Result EditTaskName(int index, string input)
    {
        var activities = this.GetAllActivities();
        if (activities.Count == 0)
        {
            return new Result(false, "No activities found to edit");
        }

        if (index >= activities.Count)
        {
            return new Result(false, "Index out of range");
        }

        var activityToEdit = activities.ElementAt(index);
        activityToEdit.ActivityName = input;
        this._activityRepository.EditActivity(activityToEdit.ActivityId, activityToEdit);
        return new Result(true, "Edit Successfull!");
    }

    internal void UpdateTaskTotalDuration(Activity activity, TimeSpan totalDuration)
    {
        throw new NotImplementedException();
    }
}
