using System.Collections.Generic;
using System.Linq;
using TimerApplication.Domain.Model;
using TimerApplication.InfrastructureLayer;

namespace TimerApplication.ApplicationLayer.Service
{
    public class ActivityService
    {
        private ActivityRepository _activityRepository;

        public ActivityService(ActivityRepository activityRepository)
        {
            this._activityRepository = activityRepository;
        }

        public void CreateNewTask(Activity activity)
        {
            this._activityRepository.CreateActivity(activity);
        }

        public List<Activity> GetAllActivities()
        {
            return this._activityRepository.GetAllActivities().ToList();
        }
    }
}
