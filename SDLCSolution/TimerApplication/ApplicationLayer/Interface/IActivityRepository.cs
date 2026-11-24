using TimerApplication.Domain.Model;

namespace TimerApplication.ApplicationLayer.Interface;

public interface IActivityRepository
{
    public void CreateActivity(Activity activity);

    public IEnumerable<Activity> GetAllActivitiesByUser(Guid userId);  
}
