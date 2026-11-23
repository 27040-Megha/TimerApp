using System;
using System.Collections.Generic;
using System.Linq;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Service;

public class ActivityService
{
    private IActivityRepository _activityRepository;

    public ActivityService(IActivityRepository activityRepository)
    {
        this._activityRepository = activityRepository;
    }

    public void CreateNewTask(Activity activity)
    {
        this._activityRepository.CreateActivity(activity);
    }

        public List<Activity> GetAllActivities()
        {
            return this._activityRepository.GetAllActivities().Where(a => a.UserId == CurrentSessionManager.CurrentSessionUser.UserId).ToList();
        }

        public void EditTaskDuration(ActivityLog activityLog)
        {
            Activity activity = this.GetAllActivities().FirstOrDefault(activity => activity.ActivityId == activityLog.ActivityId);
            activity.TotalDuration += activityLog.Duration;
            activity.TaskStatus = TimerStatus.Stopped;
            this._activityRepository.EditActivity(activityLog.ActivityId, activity);
        }

        public void EditTaskStatus(Guid taskId, TimerStatus timerStatus)
        {
            Activity activity = this.GetAllActivities().FirstOrDefault(activity => activity.ActivityId == taskId);
            activity.TaskStatus = timerStatus;
            this._activityRepository.EditActivity(taskId, activity);
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
    public void UpdateTaskTotalDuration(Activity activitySelected, TimeSpan totalTaskDuration)
    {
        var activity = this.GetAllActivities().FirstOrDefault(a => a.ActivityId == activitySelected.ActivityId);
        activity.TotalDuration += totalTaskDuration;
        this._activityRepository.EditActivity(activity.ActivityId, activity);
    }
}
