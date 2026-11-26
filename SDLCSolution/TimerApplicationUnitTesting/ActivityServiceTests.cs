using NSubstitute;
using NUnit.Framework;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;

namespace TimerApplicationUnitTesting;

[TestFixture]
public class ActivityServiceTests
{
    private IActivityRepository _mockActivityRepository = null!;
    private IActivityLogRepository _mockActivityLogRepository = null!;
    private ISessionManager _mockSessionManager = null!;
    private ActivityLogService _activityLogService = null!;
    private ActivityService _activityService = null!;
    private Guid _userId;

    [SetUp]
    public void SetUp()
    {
        _mockActivityRepository = Substitute.For<IActivityRepository>();
        _mockActivityLogRepository = Substitute.For<IActivityLogRepository>();
        _mockSessionManager = Substitute.For<ISessionManager>();
        _userId = Guid.NewGuid();
        _mockSessionManager.CurrentSessionUser.Returns(new User
        {
            UserId = _userId
        });

        _activityService = new ActivityService(_mockActivityRepository, _mockSessionManager);
        _activityLogService = new ActivityLogService(_mockActivityLogRepository, _activityService, _mockSessionManager);
    }

    [Test]
    public void DeleteActivity_ShouldReturnFailure_WhenNoActivitiesExist()
    {
        Result actualResult = _activityLogService.DeleteActivity(2);
        Assert.Multiple(() =>
        {
            Assert.That(actualResult.IsSuccess, Is.False);
            Assert.That(actualResult.ErrorType, Is.EqualTo(ErrorCode.NoActivitiesFound));
        });
    }

    [Test]
    public void DeleteActivity_ShouldReturnFailure_WhenIndexIsNegative()
    {
        AddSampleActivity();
        Result actualResult = _activityLogService.DeleteActivity(-1);
        Assert.Multiple(() =>
        {
            Assert.That(actualResult.IsSuccess, Is.False);
            Assert.That(actualResult.ErrorType, Is.EqualTo(ErrorCode.IndexOutOfRange));
        });
    }

    [Test]
    public void DeleteActivity_ShouldReturnFailure_WhenIndexIsOutOfBoundsOfActivityList()
    {
        AddSampleActivity();
        Result actualResult = _activityLogService.DeleteActivity(5);
        Assert.Multiple(() =>
        {
            Assert.That(actualResult.IsSuccess, Is.False);
            Assert.That(actualResult.ErrorType, Is.EqualTo(ErrorCode.IndexOutOfRange));
        });
    }

    [Test]
    public void DeleteActivity_ShouldReturnFailure_WhenTimerIsRunning()
    {
        AddSampleActivity(TimerStatus.Running);
        Result actualResult = _activityLogService.DeleteActivity(0);
        Assert.Multiple(() =>
        {
            Assert.That(actualResult.IsSuccess, Is.False);
            Assert.That(actualResult.ErrorType, Is.EqualTo(ErrorCode.TimerRunning));
        });
    }

    [Test]
    public void DeleteActivity_ShouldReturnSuccess_WhenTimerIsStopped()
    {
        Activity sampleActivity = AddSampleActivity();
        Result actualResult = _activityLogService.DeleteActivity(0);
        Assert.Multiple(() =>
        {
            Assert.That(actualResult.IsSuccess, Is.True);
            Assert.That(actualResult.ErrorType, Is.EqualTo(ErrorCode.None));
            _mockActivityRepository.Received(1).DeleteActivity(sampleActivity.ActivityId);
        });
    }

    [Test]
    public void DeleteActivity_ShouldDeleteOnlyTargetedActivity_WhenMultipleActivitiesExist()
    {
        Activity firstActivity = new Activity("Coding", TimeSpan.Zero, _userId);
        Activity secondActivity = new Activity("Reading", TimeSpan.Zero, _userId);
        Activity thirdActivity = new Activity("Exercise", TimeSpan.Zero, _userId);
        _mockActivityRepository.GetAllActivitiesByUser(_userId).Returns(new List<Activity>
        {
            firstActivity,
            secondActivity,
            thirdActivity
        });
        Result actualResult = _activityLogService.DeleteActivity(1);
        Assert.Multiple(() =>
        {
            Assert.That(actualResult.IsSuccess, Is.True);
            Assert.That(actualResult.ErrorType, Is.EqualTo(ErrorCode.None));
            _mockActivityRepository.Received(1).DeleteActivity(secondActivity.ActivityId);
            _mockActivityRepository.DidNotReceive().DeleteActivity(firstActivity.ActivityId);
            _mockActivityRepository.DidNotReceive().DeleteActivity(thirdActivity.ActivityId);
        });
    }

    private Activity AddSampleActivity(TimerStatus logStatus = TimerStatus.Stopped)
    {
        Activity activity = new Activity("Coding", TimeSpan.Zero, _userId);
        _mockActivityRepository.GetAllActivitiesByUser(_userId).Returns(new List<Activity> { activity });
        ActivityLog activityLog = new ActivityLog
        {
            ActivityId = activity.ActivityId,
            UserId = _userId,
            LogStatus = logStatus
        };

        _mockActivityLogRepository.FetchAllActivityLogs().Returns(new List<ActivityLog> { activityLog });
        return activity;
    }
}
