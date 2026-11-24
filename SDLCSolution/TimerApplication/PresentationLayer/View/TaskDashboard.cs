using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.Domain.Enums;
using TimerApplication.PresentationLayer.Helper;
using TimerApplication.Domain.Model;

namespace TimerApplication.PresentationLayer.View;

public class TaskDashboard
{
    // Left Side (Displays menu from position 0 to 54), Right Side (Displays Current User's task from Position 55)
    private const int _activityPanelLeft = 55;

    private readonly UserService _userService;

    private readonly ISessionManager _currentSessionManager;

    private readonly IUserIO _userIO;

    private readonly ActivityService _activityService;

    private List<Activity> _cachedActivities = [];

    public TaskDashboard(UserService userService, ActivityService activityService, IUserIO userIO, ISessionManager currentSessionManager)
    {
        this._userService = userService;
        this._activityService = activityService;
        this._userIO = userIO;
        this._currentSessionManager = currentSessionManager;
    }

    public void RunDashboard()
    {
        _userIO.ClearOutput();
        _cachedActivities = this.GetAllActivities();
        DisplayCurrentUsersTask();
        _userIO.DisplayMessage($"Welcome {_currentSessionManager.CurrentSessionUser?.UserName}", MessageType.Info);
        using System.Timers.Timer dashboardTimer = new(1000);
        dashboardTimer.Elapsed += (_, _) =>
        {
            try
            {
                DisplayCurrentUsersTask();
            }
            catch
            {
                _userIO.DisplayMessage("Error in displaying current User's task", MessageType.Warning);
            }
        };
        dashboardTimer.Start();
        try
        {
            int choice;
            do
            {
                this.DisplayMenu();
                if (!InputValidator.ValidateInteger(_userIO.ReadInput(), out choice))
                {
                    _userIO.DisplayMessage("Invalid Integer format, Enter a valid integer!", MessageType.Error);
                    continue;
                }
                this.HandleMenu(choice);
                _cachedActivities = this.GetAllActivities();
                DisplayCurrentUsersTask();
            }
            while (choice != 9);
        }
        finally
        {
            dashboardTimer.Stop();
        }
    }

    private void DisplayMenu()
    {
        _userIO.DisplayMessage("1. Add new Task \n" +
                "2. Edit Task\n" +
                "3. Delete Task\n" +
                "4. View Specific Task Log\n" +
                "5. Start Timer\n" +
                "6. Stop Timer\n" +
                "7. Import Time log\n" +
                "8. Export Time logs\n" +
                "9. Logout\n" +
                "Enter your option (1 to 9): "
                );
    }

    private void HandleMenu(int choice)
    {
        switch (choice)
        {
            case 1:
                this.AddNewTask();                   
                break;
            case 2:
                this.EditTask();
                break;
            case 3:
                this.DeleteTask();
                break;
            case 4:
                this.ViewSpecificTaskLog();
                break;
            case 5:
                this.StartTimer();
                break;
            case 6:
                this.StopTimer();
                break;
            case 7:
                this.ImportTimeLog();
                break;
            case 8:
                this.ExportTimeLog();
                break;
            case 9:
                this.Logout();
                break;
            default:
                _userIO.DisplayMessage("Invalid Choice, Enter a valid choice between 1 to 9", MessageType.Error);
                break;
        }
    }

    private void Logout()
    {
        _userService.ClearSession();
        _userIO.DisplayMessage("Logging out", MessageType.Info);
    }

        private void AddNewTask()
        {
            if (!TryGetTaskName(out string? taskName))
            {
                _userIO.DisplayMessage($"Invalid string format, Task name should not be null or empty!", MessageType.Error);
                return;
            }

            this._activityService.CreateNewTask(new Activity(taskName, TimeSpan.Zero, _currentSessionManager.CurrentSessionUser?.UserId));
            _userIO.DisplayMessage("Task Added Successfully!", MessageType.Success);
        }

        private bool TryGetTaskName(out string? result)
        {
            _userIO.DisplayMessage("Enter Task Name: ");
            result = _userIO.ReadInput()?.Trim();
            if (!InputValidator.ValidateString(result))
            {
                return false;
            }

            return true;
        }

    private void DisplayCurrentUsersTask()
    {
        if (_currentSessionManager.CurrentSessionUser == null) return;

        int width = _userIO.WindowWidth - _activityPanelLeft - 1;
        int maxRows = _userIO.WindowHeight - 3;
        if (width <= 10 || maxRows <= 0) return;

        _userIO.DisplayMessageAt(_activityPanelLeft, 0, "Your activities: ", MessageType.Info);

        List<Activity> activities = _cachedActivities;
        int visibleCount = Math.Min(activities.Count, maxRows);

        for (int i = 0; i < visibleCount; i++)
        {
            TimeSpan duration = activities[i].TotalDuration;
            string formattedDuration = $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
            string text = $"{i + 1}. {activities[i].ActivityName}  Duration: {formattedDuration}";
            _userIO.DisplayMessageAt(_activityPanelLeft, i + 1, text.PadRight(width));
        }

        string footer = activities.Count > maxRows ? $"... and {activities.Count - maxRows} more" : string.Empty;
        _userIO.DisplayMessageAt(_activityPanelLeft, visibleCount + 1, footer.PadRight(width));
    }


    private List<Activity> GetAllActivities()
    {
        User? currentUser = _currentSessionManager.CurrentSessionUser;
        if (currentUser == null)
        {
            return [];
        }

        return this._activityService.GetAllActivitiesByUser(currentUser.UserId); 
    }

    private void EditTask()
    {
        _userIO.DisplayMessage("Not available yet", MessageType.Warning);
    }

    private void DeleteTask()
    {
        _userIO.DisplayMessage("Not available yet", MessageType.Warning);
    }

    private void ViewSpecificTaskLog()
    {
        _userIO.DisplayMessage("Not available yet", MessageType.Warning);
    }

    private void StartTimer()
    {
        _userIO.DisplayMessage("Not available yet", MessageType.Warning);
    }

    private void StopTimer()
    {
        _userIO.DisplayMessage("Not available yet", MessageType.Warning);
    }

    private void ImportTimeLog()
    {
        _userIO.DisplayMessage("Not available yet", MessageType.Warning);
    }

    private void ExportTimeLog()
    {
        _userIO.DisplayMessage("Not available yet", MessageType.Warning);
    }
}