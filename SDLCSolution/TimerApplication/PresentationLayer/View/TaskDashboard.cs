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

    private readonly ActivityLogService _activityLogService;

    private List<Activity> _cachedActivities = [];

    private List<ActivityLog> _cachedActiveLogs = [];

    public TaskDashboard(UserService userService, ActivityService activityService, IUserIO userIO, ActivityLogService activityLogService, ISessionManager currentSessionManager)
    {
        this._userService = userService;
        this._activityService = activityService;
        this._userIO = userIO;
        this._activityLogService = activityLogService;
        this._currentSessionManager = currentSessionManager;
    }

    public void RunDashboard()
    {
        _userIO.ClearOutput();
        _cachedActivities = this.GetAllActivities();
        _cachedActiveLogs = this._activityLogService.GetAllActiveTimerLogs().ToList();
        DisplayCurrentUsersTask();
        _userIO.DisplayMessage($"Welcome {_currentSessionManager.CurrentSessionUser?.UserName}", MessageType.Info);
        using System.Timers.Timer dashboardTimer = new(1000);
        dashboardTimer.Elapsed += (_, _) =>
        {
            try
            {
                DisplayCurrentUsersTask();
                DisplayActiveTimers();
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
                _cachedActiveLogs = this._activityLogService.GetAllActiveTimerLogs().ToList();
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
        _userIO.DisplayMessage("\nMENU OPTIONS", MessageType.Info);
        _userIO.DisplayPrompt("1. Add new Task \n" +
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
                this.Edit();
                break;
            case 3:
                this.Delete();
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

    private void Edit()
    {
        if (!HasAnyItems(GetAllActivities()))
        {
            _userIO.DisplayMessage("No activities found to Edit", MessageType.Warning);
            return;
        }

            _userIO.DisplayMessage("1. Edit Task \n2. Edit Task Logs \n3. Back \nEnter Your Option (1/2): ");
            if (!InputValidator.ValidateInteger(_userIO.ReadInput(), out int choice))
            {
                _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
                return;
            }
            switch (choice)
            {
                case 1:
                    this.EditTask();
                    break;
                case 2:
                    this.EditTaskLog();
                    break;
                case 3:
                    break;
                default:
                    _userIO.DisplayMessage("Invalid Choice, Enter a valid choice between 1 or 2", MessageType.Error);
                    break;
            }
        }

    private void Delete()
    {
        if (!HasAnyItems(GetAllActivities()))
        {
            _userIO.DisplayMessage("No activities found to Delete", MessageType.Warning);
            return;
        }

        _userIO.DisplayMessage("1. Delete Task \n2. Delete Task Logs \n3. Back \nEnter Your Option (1/2): ");
        if (!InputValidator.ValidateInteger(_userIO.ReadInput(), out int choice))
        {
            _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
            return;
        }
        switch (choice)
        {
            case 1:
                this.DeleteTask();
                break;
            case 2:
                this.DeleteTaskLogs();
                break;
            case 3:
                break;
            default:
                _userIO.DisplayMessage("Invalid Choice, Enter a valid choice 1 or 2", MessageType.Error);
                break;
        }
    }


    private void DeleteTask()
    {
        List<Activity> activities = this.GetAllActivities();
        if (!HasAnyItems(activities))
        {
            _userIO.DisplayMessage("No activities found to delete!", MessageType.Warning);
            return;
        }

        this.DisplayActivities(activities);
        if (!TrySelectIndex("Enter index of task to be deleted: ",activities, out int index))
        {
            return;
        }

        Result deletionResult = this._activityLogService.DeleteActivity(activities[index].ActivityId);
        if (!deletionResult.IsSuccess)
        {
            string errorMessage = this.GetErrorMessage(deletionResult.ErrorType);
            _userIO.DisplayMessage(errorMessage, MessageType.Error);
            return;
        }

        _userIO.DisplayMessage("Task Deleted Successfully!", MessageType.Success);
    }

    private string GetErrorMessage(ErrorCode errorCode)
    {
        string errorMessage = errorCode switch
        {
            ErrorCode.NoActivitiesFound => "No activities found to delete",
            ErrorCode.IndexOutOfRange => "Index out of bounds, Deletion failed",
            ErrorCode.TimerRunning => "Active timer running, Can't perform Operation",
            ErrorCode.None => "No error, Operation Successful",
            _ => "Deletion failed due to some unknown reasons"
        };

        return errorMessage;
    }

    private void DeleteTaskLogs()
    {
        List<ActivityLog> activityLogs = this._activityLogService.GetAllActivityLogs().ToList();
        if (!HasAnyItems(activityLogs))
        {
            _userIO.DisplayMessage("No activity logs found to delete!", MessageType.Warning);
            return;
        }

        this.DisplayActivityLogs(activityLogs);
        if (!TrySelectIndex("Enter index of task log to be deleted: ",activityLogs, out int index))
        {
            return;
        }

        Result deletionResult = this._activityLogService.DeleteActivityLogByLogId(activityLogs[index].ActivityLogId);
        DisplayResult(deletionResult, "Activity Log Deleted Successfully!");
    }

    private void EditTaskLog()
    {
        List<ActivityLog> activityLogs = this._activityLogService.GetAllActivityLogs().ToList();
        if (!HasAnyItems(activityLogs))
        {
            _userIO.DisplayMessage("No activity logs found to edit!", MessageType.Warning);
            return;
        }

        this.DisplayActivityLogs(activityLogs);
        if (!TrySelectIndex("Enter index of task log to be edited: ",activityLogs, out int index))
        {
            return;
        }

        if (!TryGetTaskName(out string? logName))
        {
            _userIO.DisplayMessage("Invalid string format, Task log name should not be null or empty!", MessageType.Error);
            return;
        }

        Result editResult = this._activityLogService.EditTaskLog(activityLogs[index].ActivityLogId, logName!);
        DisplayResult(editResult, "Task log name edited successfully!");
    }

    private void EditTask()
    {
        List<Activity> activities = this.GetAllActivities();
        if (!HasAnyItems(activities))
        {
            _userIO.DisplayMessage("No activities found to edit!", MessageType.Warning);
            return;
        }

        this.DisplayActivities(activities);
        if (!TrySelectIndex("Enter index of task to be edited: ",activities, out int index))
        {
            return;
        }

        if (!TryGetTaskName(out string? taskName))
        {
            _userIO.DisplayMessage("Invalid string format, Task name should not be null or empty!", MessageType.Error);
            return;
        }

        Result editResult = this._activityService.EditTaskName(activities[index].ActivityId, taskName!);
        DisplayResult(editResult, "Task name edited successfully!");
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
        if (_currentSessionManager.CurrentSessionUser == null)
        {
            return;
        }

        List<string> formattedActivities = _cachedActivities
            .Select((activity, index) =>
            {
                TimeSpan cumulativeDuration = this._activityLogService.GetCurrentActivityDuration(activity, _cachedActiveLogs);
                return $"{index + 1}. {activity.ActivityName}  Duration: {FormatDuration(cumulativeDuration)}";
            })
            .ToList();

        _userIO.DisplayPanel(DisplayRegion.PrimarySidebar, "Your activities:", formattedActivities);
    }


    private void DisplayActiveTimers()
    {
        List<string> lines = _cachedActiveLogs
            .Select((log, i) =>
            {
                string duration = (DateTime.UtcNow - log.StartTime).ToString(@"hh\:mm\:ss");
                return $"{i + 1}. {log.LogName} Start: {log.StartTime.ToLocalTime()} Duration: {duration}";
            })
            .ToList();

        _userIO.DisplayPanel(DisplayRegion.SecondarySidebar, "Active Timers: ", lines);
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

    private void ViewSpecificTaskLog()
    {
        List<Activity> activities = this.GetAllActivities();
        if (!HasAnyItems(activities))
        {
            _userIO.DisplayMessage("No activities found to view specific logs!", MessageType.Warning);
            return;
        }

        this.DisplayActivities(activities);
        if (!TrySelectIndex("Enter index of task to view activity details: ",activities, out int index))
        {
            return;
        }

        Result<ActivityLog> logsResult = this._activityLogService.GetAllActivityLogsByActivityIndex(activities[index].ActivityId);
        if (!logsResult.IsSuccess)
        {
            _userIO.DisplayMessage(logsResult.Message, MessageType.Error);
            return;
        }

        Activity activity = activities[index];
        _userIO.DisplayMessage($"{activity.ActivityName}  Duration: {FormatDuration(activity.TotalDuration)}", MessageType.Info);
        this.DisplayLogDetails(logsResult.Data);
    }


    private void DisplayLogDetails(IEnumerable<ActivityLog> activityLogs)
    {
        List<ActivityLog> logsList = activityLogs.ToList();
        for (int i = 0; i < logsList.Count; i++)
        {
            ActivityLog log = logsList[i];
            bool isRunning = log.LogStatus == TimerStatus.Running;
            string durationText = isRunning ? string.Empty : $" (Duration: {FormatDuration(log.Duration)})";
            string? endText = isRunning ? "Running" : log.EndTime?.ToLocalTime().ToString();
            MessageType messageType = isRunning ? MessageType.Success : MessageType.Default;

            _userIO.DisplayMessage(
                $"{i + 1}. {log.LogName}{durationText}\n" +
                $"   Start: {log.StartTime.ToLocalTime()},  End: {endText}",
                messageType);
        }
    }

    private bool HasAnyItems<T>(IReadOnlyList<T> entries)
    {
        return entries.Any();
    }

    private void StartTimer()
    {
        IReadOnlyList<Activity> activities = this.GetAllActivities().ToList();
        if (!HasAnyItems(activities))
        {
            _userIO.DisplayMessage("No activities found to start the timer!", MessageType.Warning);
            return;
        }

        this.DisplayActivities(activities);
        if (!TrySelectIndex("Enter index of task to Start Timer: ", activities, out int index))
        {
            return;
        }

        Result result = this._activityLogService.StartTimer(activities[index].ActivityId);
        DisplayResult(result, "Timer started Successfully!");
    }

    private void StopTimer()
    {
        IReadOnlyList<ActivityLog> activeLogs = this._activityLogService.GetAllActiveTimerLogs().ToList();
        if (!HasAnyItems(activeLogs))
        {
            _userIO.DisplayMessage("No active logs found to stop the timer!", MessageType.Warning);
            return;
        }

        this.DisplayActivityLogs(activeLogs);
        if (!TrySelectIndex("Enter index of task to Stop Timer: ", activeLogs, out int index))
        {
            return;
        }

        Result result = this._activityLogService.StopTimer(activeLogs[index].ActivityLogId);
        DisplayResult(result, "Timer stopped successfully!");
    }

    private bool TrySelectIndex<T>(string prompt, IEnumerable<T> collection, out int zeroBasedIndex)
    {
        zeroBasedIndex = -1;
        _userIO.DisplayMessage(prompt, MessageType.Info);

        if (!TryGetInputIndex(out int index))
        {
            _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
            return false;
        }

        if (index < 1 || index > collection.Count())
        {
            _userIO.DisplayMessage("Index Out of Range", MessageType.Error);
            return false;
        }

        zeroBasedIndex = index - 1;
        return true;
    }
    
    private bool TryGetInputIndex(string fieldName, out int index)
    {
        _userIO.DisplayPrompt($"Enter index to {fieldName}: ", MessageType.Info);
        bool isValidIndex =  InputValidator.ValidateInteger(_userIO.ReadInput(), out index);
        return isValidIndex;
    }

    private void DisplayResult(Result result, string successMessage)
    {
        if (!result.IsSuccess)
        {
            _userIO.DisplayMessage(result.Message!, MessageType.Error);
            return;
        }

        _userIO.DisplayMessage(successMessage, MessageType.Success);
    }

    private void DisplayActivities(IEnumerable<Activity> activities)
    {
        int i = 0;
        foreach (Activity activity in activities)
        {
            _userIO.DisplayMessage($"{++i}. {activity.ActivityName}  Duration: {FormatDuration(activity.TotalDuration)}");
        }
    }

    private bool TryGetInputIndex(out int index)
    {
        return InputValidator.ValidateInteger(_userIO.ReadInput(), out index);
    }

    private void DisplayActivityLogs(IEnumerable<ActivityLog> activityLogs)
    {
        for (int i = 0; i < activityLogs.Count(); i++)
        {
            ActivityLog activitylog = activityLogs.ElementAt(i);
            _userIO.DisplayMessage($"{i + 1}. {activitylog.LogName}");
        }
    }

    private static string FormatDuration(TimeSpan duration)
    {
        return $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
    }

    private void ImportTimeLog()
    {
        List<Activity> activities = this._activityService.GetAllActivitiesByUser(_currentSessionManager.CurrentSessionUser!.UserId);
        if (!HasAnyItems(activities))
        {
            _userIO.DisplayMessage("No activity found to import time log", MessageType.Warning);
            return;
        }

        this.DisplayActivities(activities);
        if (!TryGetInputIndex("of task to import Task logs", out int index))
        {
            _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
            return;
        }

        if (index < 1)
        { 
            _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
            return;
        }

        _userIO.DisplayMessage("Enter (.csv) file path to import logs: ", MessageType.Info);
        string? rawInput = _userIO.ReadInput();
        string? filePath = rawInput?.Trim().Trim('"').Trim();
        if (!InputValidator.ValidateString(filePath))
        {
            _userIO.DisplayMessage("File path should not be null or empty", MessageType.Error);
            return;
        }

        string? pathExtension = Path.GetExtension(filePath);
        if (!string.Equals(pathExtension, ".csv", StringComparison.OrdinalIgnoreCase))
        {
            _userIO.DisplayMessage("Invalid file path, give a valid .csv file path", MessageType.Error);
            return;
        }

        Result importResult = this._activityLogService.ImportLogs(filePath!, index-1);
        if (!importResult.IsSuccess)
        {
            _userIO.DisplayMessage(importResult.Message!, MessageType.Error);
            return;
        }

        _userIO.DisplayMessage(importResult.Message!, MessageType.Success);
    }

    private void ExportTimeLog()
    {
        List<Activity> activities = this._activityService.GetAllActivitiesByUser(_currentSessionManager.CurrentSessionUser!.UserId);
        if (!HasAnyItems(activities))
        {
            _userIO.DisplayMessage("No activity found to export time log", MessageType.Warning);
            return;
        }

        this.DisplayActivities(activities);
        if (!TryGetInputIndex("of task to export its Logs", out int index))
        {
            _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
            return;
        }

        if (index < 1 || index > activities.Count)
        {
            _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
            return;
        }

        Result exportResult = this._activityLogService.ExportLogs(index-1);
        if (!exportResult.IsSuccess)
        {
            _userIO.DisplayMessage($"{exportResult.Message}", MessageType.Error);
            return;
        }

        _userIO.DisplayMessage($"Task logs exported successfully, check in your Downloads/Desktop", MessageType.Success);
    }
}