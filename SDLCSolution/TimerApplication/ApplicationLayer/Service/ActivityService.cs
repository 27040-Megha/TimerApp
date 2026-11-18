using System.Collections.Generic;
using System.Linq;
using TimerApplication.ApplicationLayer.Interface;
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
}
