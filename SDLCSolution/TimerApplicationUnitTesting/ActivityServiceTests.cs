using NUnit.Framework;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;

namespace TimerApplicationUnitTesting;

[TestFixture]
public class ActivityServiceTests
{
    private IActivityRepository _mockActivityRepository;
    private ActivityService _activityService;
    private Guid _userId;

    private void AddSampleActivity(TimerStatus status = TimerStatus.Stopped)
    {
        Activity activity = new Activity("Coding", default, _userId)
        {
            TaskStatus = status,
        };
        _mockActivityRepository.CreateActivity(activity);
    }

    [SetUp]
    public void SetUp()
    {
        _mockActivityRepository = new MockActivityRepository();
        _activityService = new ActivityService(_mockActivityRepository);
        _userId = Guid.NewGuid();
        CurrentSessionManager.CurrentSessionUser = new User
        {
            UserId = _userId
        };
    }

    [Test]
    public void DeleteActivity_WhenNoActivitiesExist()
    {
        Result actualResult = _activityService.DeleteActivity(2);
        string expectedResult = "No activities found to delete";
        Assert.That(actualResult.IsSuccess, Is.EqualTo(false));
        Assert.That(actualResult.Message, Is.EqualTo(expectedResult));
    }

    [Test]
    public void DeleteActivity_WhenIndexIsInvalidInteger()
    {
        AddSampleActivity();
        Result actualResult = _activityService.DeleteActivity(int.Parse("ab"));
        string expectedResult = "Invalid integer format";
        Assert.That(actualResult.IsSuccess, Is.EqualTo(false));
        Assert.That(actualResult.Message, Is.EqualTo(expectedResult));
    }

    [Test]
    public void DeleteActivity_WhenIndexIsNegative()
    {
        AddSampleActivity();
        Result actualResult = _activityService.DeleteActivity(-1);
        string expectedResult = "Index out of range";
        Assert.That(actualResult.IsSuccess, Is.EqualTo(false));
        Assert.That(actualResult.Message, Is.EqualTo(expectedResult));
    }

    [Test]
    public void DeleteActivity_WhenIndexIsOutOfBoundsOfActivitylist()
    {
        AddSampleActivity();
        Result actualResult = _activityService.DeleteActivity(5);
        string expectedResult = "Index out of range";
        Assert.That(actualResult.IsSuccess, Is.EqualTo(false));
        Assert.That(actualResult.Message, Is.EqualTo(expectedResult));
    }

    [Test]
    public void DeleteActivity_WhenTimerIsRunning()
    {
        AddSampleActivity(TimerStatus.Running);
        Result actualResult = _activityService.DeleteActivity(0);
        string expectedResult = "Deletion failed, Task with active timer can't be deleted!";
        Assert.That(actualResult.IsSuccess, Is.EqualTo(false));
        Assert.That(actualResult.Message, Is.EqualTo(expectedResult));
    }

    [Test]
    public void DeleteActivity_WhenTimerIsStopped()
    {
        AddSampleActivity();
        Result actualResult = _activityService.DeleteActivity(0);
        string expectedResult = "Deletion successful!";
        Assert.That(actualResult.IsSuccess, Is.EqualTo(true));
        Assert.That(actualResult.Message, Is.EqualTo(expectedResult));
        Assert.That(_mockActivityRepository.GetAllActivities().Count(), Is.EqualTo(0));
    }
}
