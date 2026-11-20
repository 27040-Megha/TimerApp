using System;
using TimerApplication.ApplicationLayer.Domain.Enums;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.PresentationLayer.Helper;
using TimerApplication.Domain.Model;
using TimerApplication.Domain.Enums;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace TimerApplication.PresentationLayer.View
{
    public class TaskDashboard
    {
        private readonly UserService _userService;

        private readonly IUserIO _userIO;

        private readonly ActivityService _activityService;

        private readonly ActivityLogService _activityLogService;

        public TaskDashboard(UserService userService, ActivityService activityService, ActivityLogService activityLogService, IUserIO userIO)
        {
            this._userService = userService;
            this._activityService = activityService;
            this._activityLogService = activityLogService;
            this._userIO = userIO;
        }

        public void RunDashboard()
        {
            _userIO.ClearOutput();
            _userIO.DisplayMessage($"Welcome {CurrentSessionManager.CurrentSessionUser.UserName}", MessageType.Info);
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
            }
            while (choice != 9);
        }

        private void DisplayMenu()
        {
            _userIO.DisplayMessage("\nMENU OPTIONS", MessageType.Info);
            _userIO.DisplayMessageAtSameLine("1. Add new Task \n" +
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
            _userIO.DisplayMessageAtSameLine("1. Edit Task Name\n2. Edit Task Log Name \nEnter Your Option (1/2): ");
            if (!InputValidator.ValidateInteger(Console.ReadLine(), out int choice))
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
                default:
                    _userIO.DisplayMessage("Invalid Choice, Enter a valid choice between 1 or 2", MessageType.Error);
                    break;
            }
        }

        private void Delete()
        {
            _userIO.DisplayMessageAtSameLine("1. Delete Task \n2. Delete Task Logs \nEnter Your Option (1/2): ");
            if (!InputValidator.ValidateInteger(Console.ReadLine(), out int choice))
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
                default:
                    _userIO.DisplayMessage("Invalid Choice, Enter a valid choice 1 or 2", MessageType.Error);
                        break;
                }
        }

        private void DeleteTask()
        {
            var activities = this._activityService.GetAllActivities();
            this.DisplayActivities(activities);
            if (!TryGetInputIndex("delete a task", out int index))
            {
                _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
                return;
            }

            if (index < 1)
            {
                _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
                return;
            }

            Result deletionResult = this._activityService.DeleteActivity(index - 1);
            if (!deletionResult.IsSuccess)
            {
                _userIO.DisplayMessage(deletionResult.Message, MessageType.Error);
                return;
            }

            this._activityLogService.DeleteActivityLogsByTaskId(activities.ElementAt(index - 1).ActivityId);
            _userIO.DisplayMessage($"Task Deleted Successfully!", MessageType.Success);
        }

        private void DeleteTaskLogs()
        {
            var activityLogs = this._activityLogService.GetAllActivityLogs().ToList();
            this.DisplayActivityLogs(activityLogs);
            if (!TryGetInputIndex("delete a task log", out int index))
            {
                _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
                return;
            }

            if (index < 1)
            {
                _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
                return;
            }

            Result deletionResult = this._activityLogService.DeleteActivityLogByLogId(index - 1);
            if (!deletionResult.IsSuccess)
            {
                _userIO.DisplayMessage(deletionResult.Message, MessageType.Error);
                return;
            }

            _userIO.DisplayMessage($"Activity Log Deleted Successfully!", MessageType.Success);
        }

        private void EditTaskLog()
        {
            var activityLogs = this._activityLogService.GetAllActivityLogs().ToList();
            this.DisplayActivityLogs(activityLogs);
            if (!TryGetInputIndex("edit a task", out int index))
            {
                _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
                return;
            }

            if (index < 1)
            {
                _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
                return;
            }

            if (!TryGetTaskName(out string logName))
            {
                _userIO.DisplayMessage($"Invalid string format, Task log name should not be null or empty!", MessageType.Error);
                return;
            }

            Result editResult = this._activityLogService.EditTaskLog(index - 1, logName);
            if (!editResult.IsSuccess)
            {
                _userIO.DisplayMessage(editResult.Message, MessageType.Error);
                return;
            }

            _userIO.DisplayMessage($"Task log name edited successfully!", MessageType.Success);
        }


        private void EditTask()
        {
            var activities = this._activityService.GetAllActivities().ToList();
            this.DisplayActivities(activities);
            if (!TryGetInputIndex("edit task name", out int index))
            {
                _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
                return;
            }

            if (index < 1)
            {
                _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
                return;
            }

            if (!TryGetTaskName("Enter new task name", out string taskName))
            {
                _userIO.DisplayMessage($"Invalid string format, Task name should not be null or empty!", MessageType.Error);
                return;
            }

            Result editResult = this._activityService.EditTaskName(index - 1, taskName);
            if (!editResult.IsSuccess)
            {
                _userIO.DisplayMessage(editResult.Message, MessageType.Error);
                return;
            }

            _userIO.DisplayMessage($"Task name edited successfully!", MessageType.Success);
        }

        private void Logout()
        {
            CurrentSessionManager.CurrentSessionUser = null;
            _userIO.DisplayMessage("Logging out", MessageType.Info);
        }

        private void AddNewTask()
        {
            if (!TryGetTaskName("Enter task to add a new task", out string taskName))
            {
                _userIO.DisplayMessage($"Invalid string format, Task name should not be null or empty!", MessageType.Error);
                return;
        }

            this._activityService.CreateNewTask(new Activity(taskName, TimeSpan.Zero, CurrentSessionManager.CurrentSessionUser.UserId));
            _userIO.DisplayMessage("Task Added Successfully!", MessageType.Success);
        }

        private bool TryGetTaskName(string message, out string result)
        {
            _userIO.DisplayMessage($"{message}: ");
            result = _userIO.ReadInput();
            if (!InputValidator.ValidateString(result))
            {
                return false;
        }

            return true;
        }

        private void ViewSpecificTaskLog()
        {
            List<Activity> activities = this._activityService.GetAllActivities();
            this.DisplayActivities(activities);

            this.DisplayActivities(activities);
            if (!TryGetInputIndex("view task details", out int index))
            {
                _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
                return;
            }

            if (index < 1)
            {
                _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
                return;
            }

            Result isLogsFetched = this._activityLogService.GetAllActivityLogsByActivityIndex(index - 1, out IEnumerable<ActivityLog> activityLogs);
            if (!isLogsFetched.IsSuccess)
            {
                _userIO.DisplayMessage(isLogsFetched.Message, MessageType.Error);
                return;
            }

            Activity activity = activities.ElementAt(index - 1);
            _userIO.DisplayMessage($"{activity.ActivityName}  Duration: {activity.TotalDuration.ToString(@"hh\:mm\:ss")}", MessageType.Info);
            this.DisplayLogDetails(activityLogs);
        }

        private void DisplayLogDetails(IEnumerable<ActivityLog> activityLogs)
        {
            var logsList = activityLogs.ToList();
            for (int i = 0; i < logsList.Count; i++)
            {
                var log = logsList[i];
                bool isRunning = log.LogStatus == TimerStatus.Running;

                TimeSpan elapsed = isRunning ? (DateTime.Now - log.StartTime) : log.Duration;
                string duration = elapsed.ToString(@"hh\:mm\:ss");
                string endText = isRunning ? "Running" : log.EndTime?.ToString();
                MessageType messageType = isRunning ? MessageType.Success : MessageType.Default;

                _userIO.DisplayMessage(
                    $"{i + 1}. {log.LogName}, Duration: {duration}\n" +
                    $"   Start: {log.StartTime}\n"+
                    $"   End: {endText}",
                    messageType);
            }
        }


        private void StartTimer()
        {
            IEnumerable<Activity> activities = this.GetAllActivities();
            this.DisplayActivities(activities);
            if (!TryGetInputIndex("Start timer for task", out int index))
            {
                _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
                return;
            }

            if (index < 1)
            {
                _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
                return;
            }

            index = index - 1;
            Result startTimerResult = this._activityLogService.StartTimer(index);
            if (!startTimerResult.IsSuccess)
            {
                _userIO.DisplayMessage(startTimerResult.Message, MessageType.Error);
                return;
            }

            _userIO.DisplayMessage($"Timer started Successfully!", MessageType.Success);
        }

        private IEnumerable<Activity> GetAllActivities()
        {
            return this._activityService.GetAllActivities();
        }

        private void DisplayActivities(IEnumerable<Activity> activities)
        {
            for (int i = 0; i < activities.Count(); i++)
            {
                var activity = activities.ElementAt(i);
                _userIO.DisplayMessage($"{i + 1}. {activity.ActivityName}  Duration: {activity.TotalDuration.ToString(@"hh\:mm\:ss")}");
            }
        }

        private bool TryGetInputIndex(string fieldName, out int index)
        {
            _userIO.DisplayMessageAtSameLine($"Enter index to {fieldName}: ", MessageType.Info);
            bool isValidIndex =  InputValidator.ValidateInteger(_userIO.ReadInput(), out index);
            return isValidIndex;
        }

        private void StopTimer()
        {
            IEnumerable<ActivityLog> activeLogs = this._activityLogService.GetAllActiveTimerLogs();
            this.DisplayActivityLogs(activeLogs);
            _userIO.DisplayMessage($"Enter index of task to Stop Timer: ", MessageType.Info);
            if (!TryGetInputIndex(out int index))
            {
                _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
                return;
            }

            if (index < 1)
            {
                _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
                return;
            }

            index = index - 1;
            Result stopTimerResult = this._activityLogService.StopTimer(index);
            if (!stopTimerResult.IsSuccess)
            {
                _userIO.DisplayMessage(stopTimerResult.Message, MessageType.Error);
                return;
            }

            _userIO.DisplayMessage($"Timer stopped successfully!", MessageType.Success);
        }

        private void DisplayActivityLogs(IEnumerable<ActivityLog> activityLogs)
        {

            for (int i = 0; i < activityLogs.Count(); i++)
            {
                var activitylog = activityLogs.ElementAt(i);
                _userIO.DisplayMessage($"{i + 1}. {activitylog.LogName}");
            }
        }

        private IEnumerable<ActivityLog> GetAllActivityLogs()
        {
            return this._activityLogService.GetAllActivityLogs();
        }

        private void ImportTimeLog()
        {
            var activities = this._activityService.GetAllActivities();
            if (!activities.Any())
            {
                _userIO.DisplayMessage("No tasks added to import timelogs!", MessageType.Warning);
                return;
            }

            this.DisplayActivities(activities);
            if (!TryGetInputIndex("of task to import Task logs", out int index))
            {
                _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
                return;
            }

            if (index < 1 || index > activities.Count)
            {
                _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
                return;
            }

            _userIO.DisplayMessage("Enter (.csv) file path to import logs: ", MessageType.Info);
            var filePath = Console.ReadLine();
            if (!InputValidator.ValidateString(filePath))
            {
                _userIO.DisplayMessage("File path should not be null or empty", MessageType.Error);
                return;
            }

            if (!Path.GetExtension(filePath).Equals(".csv",  StringComparison.OrdinalIgnoreCase))
            {
                _userIO.DisplayMessage("Invalid file path, give a valid .csv file path", MessageType.Error);
                return;
            }

            var importResult = this._activityLogService.ImportLogs(filePath, index-1);
            if (!importResult.IsSuccess)
            {
                _userIO.DisplayMessage(importResult.Message, MessageType.Error);
                return;
            }

            _userIO.DisplayMessage(importResult.Message, MessageType.Success);
        }

        private void ExportTimeLog()
        {
            var activities = this._activityService.GetAllActivities();
            if (!activities.Any())
            {
                _userIO.DisplayMessage("No tasks added to export timelogs!", MessageType.Warning);
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

            var exportResult = this._activityLogService.ExportLogs(index-1);
            if (!exportResult.IsSuccess)
            {
                _userIO.DisplayMessage($"{exportResult.Message}", MessageType.Error);
                return;
            }

            _userIO.DisplayMessage($"Task logs exported successfully, check in your Downloads", MessageType.Success);
        }
    }
}
