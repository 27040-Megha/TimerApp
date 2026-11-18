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
    }
