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
    public void DeleteActivity_ShallReturnFailure_WhenNoActivitiesExist()
    {
        Result actualResult = _activityService.DeleteActivity(2);
        Assert.That(actualResult.IsSuccess, Is.EqualTo(false));
        Assert.That(actualResult.ErrorType, Is.EqualTo(ErrorCode.NoActivitiesFound));
    }

    [Test]
    public void DeleteActivity_ThrowsFormatException_WhenIndexIsInvalidInteger()
    {
        AddSampleActivity();
        Assert.Throws<FormatException>(() => _activityService.DeleteActivity(int.Parse("ab")));
    }

    [Test]
    public void DeleteActivity_ThrowsArgumentOutOfRangeException_WhenIndexIsNegative()
    {
        AddSampleActivity();
        Assert.Throws<ArgumentOutOfRangeException>(() => _activityService.DeleteActivity(-1));
    }

    [Test]
    public void DeleteActivity_ShallReturnFailure_WhenIndexIsOutOfBoundsOfActivitylist()
    {
        AddSampleActivity();
        Result actualResult = _activityService.DeleteActivity(5);
        Assert.That(actualResult.IsSuccess, Is.EqualTo(false));
        Assert.That(actualResult.ErrorType, Is.EqualTo(ErrorCode.IndexOutOfRange));
    }

    [Test]
    public void DeleteActivity_ShallReturnFailure_WhenTimerIsRunning()
    {
        AddSampleActivity(TimerStatus.Running);
        Result actualResult = _activityService.DeleteActivity(0);
        Assert.That(actualResult.IsSuccess, Is.EqualTo(false));
        Assert.That(actualResult.ErrorType, Is.EqualTo(ErrorCode.TimerRunning));
    }

    [Test]
    public void DeleteActivity_ShallReturnSuccess_WhenTimerIsStopped()
    {
        AddSampleActivity();
        Result actualResult = _activityService.DeleteActivity(0);
        Assert.That(actualResult.IsSuccess, Is.EqualTo(true));
        Assert.That(actualResult.ErrorType, Is.EqualTo(ErrorCode.None));
        Assert.That(_mockActivityRepository.GetAllActivities().Count(), Is.EqualTo(0));
    }
}
