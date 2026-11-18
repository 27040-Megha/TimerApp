using System;
using System.Collections.Generic;
using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Interface;

public interface IActivityRepository
{
    public void CreateActivity(Activity activity);

    public void EditActivity(Guid activityIdToEdit, Activity activity);

    public void DeleteActivity(Guid ActivityId);

    public IEnumerable<Activity> GetAllActivities();

    public Activity GetActivityById(Guid activityId);
}
