using System;
using System.Collections.Generic;
using System.Linq;
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
    }
    public List<Activity> GetAllActivitiesByUser(Guid userId)
    {
        return this._activityRepository.GetAllActivitiesByUser(userId).ToList();
    }
}
