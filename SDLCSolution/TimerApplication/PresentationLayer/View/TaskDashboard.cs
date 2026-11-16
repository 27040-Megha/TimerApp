using System;
using TimerApplication.ApplicationLayer.Domain.Enums;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.PresentationLayer.Helper;
using TimerApplication.Domain.Model;
using TimerApplication.Domain.Enums;
using System.Linq;
using System.Collections.Generic;

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
                    _userIO.DisplayMessage("Invalid Choice, Enter a valid choice between 1 to 3", MessageType.Error);
                        break;
                }
            }
            while (choice != 9);
        }

        private void Edit()
        {
            int choice;
            Console.WriteLine("1. Edit Task");
            Console.WriteLine("2. Edit Task Logs");
            Console.WriteLine("Enter Your Option (1/2): ");
            bool isValidChoice = InputValidator.ValidateInteger(Console.ReadLine(), out choice);
            if (!isValidChoice)
            {
                TextColor.WriteColoredLine($"Invalid Integer format, Enter a valid integer!", ConsoleColor.Red);
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
                    TextColor.WriteColoredLine("Invalid Choice, Enter a valid choice between 1 or 2", ConsoleColor.Red);
                    break;
            }
        }

        private void Delete()
        {
            int choice;
            Console.WriteLine("1. Delete Task");
            Console.WriteLine("2. Delete Task Logs");
            Console.WriteLine("Enter Your Option (1/2): ");
            bool isValidChoice = InputValidator.ValidateInteger(Console.ReadLine(), out choice);
            if (!isValidChoice)
            {
                TextColor.WriteColoredLine($"Invalid Integer format, Enter a valid integer!", ConsoleColor.Red);
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
                    TextColor.WriteColoredLine("Invalid Choice, Enter a valid choice between 1 or 2", ConsoleColor.Red);
                    break;
            }
        }

        private void DisplayActivities(List<Activity> activities)
        {
            for (int i = 0; i < activities.Count; i++)
            {
                var task = activities[i];
                Console.WriteLine($"{i + 1}. {task.ActivityName}  Duration: {task.TotalDuration.ToString(@"hh\:mm\:ss")}");
            }
        }

        private void DeleteTask()
        {
            var activities = this._activityService.GetAllActivities();
            this.DisplayActivities(activities);
            TextColor.WriteColoredLine($"Enter index of task to be deleted: ", ConsoleColor.Cyan);
            int index;
            bool isValidIndex = InputValidator.ValidateInteger(Console.ReadLine(), out index);
            if (index < 1 || index > activities.Count)
            {
                TextColor.WriteColoredLine($"Index Out of Range", ConsoleColor.Red);
                return;
            }

            var activityToDelete = activities[index - 1];
            if (activityToDelete.TaskStatus == TimerStatus.Running)
            {
                TextColor.WriteColoredLine($"Task with Active timer can't be deleted, Stop the timer first!", ConsoleColor.Red);
                return;
            }

            this._activityLogService.DeleteActivityLogsByTaskId(activityToDelete.ActivityId);
            this._activityService.DeleteActivity(activityToDelete.ActivityId);
            TextColor.WriteColoredLine($"Task Deleted Successfully!", ConsoleColor.Green);
        }

        private void DeleteTaskLogs()
        {
            var activityLogs = this._activityLogService.GetAllActivityLogs();
            this.DisplayActivityLogs(activityLogs);
            TextColor.WriteColoredLine($"Enter index of task log to be deleted: ", ConsoleColor.Cyan);
            int index;
            bool isValidIndex = InputValidator.ValidateInteger(Console.ReadLine(), out index);
            if (index < 1 || index > activityLogs.Count)
            {
                TextColor.WriteColoredLine($"Index Out of Range", ConsoleColor.Red);
                return;
            }

            index = index - 1;
            var activityLogToBeDeleted = activityLogs[index];
            bool deletionStatus = this._activityLogService.DeleteActivityLogByLogId(activityLogToBeDeleted.ActivityLogId);           
            if (!deletionStatus)
            {
                TextColor.WriteColoredLine($"Cannot delete a log while running timer!, Stop the timer first", ConsoleColor.Red);
                return;
            }

            this._activityService.RemoveLogDuration(activityLogToBeDeleted.TaskId, activityLogToBeDeleted.Duration);
            TextColor.WriteColoredLine($"Activity Log Deleted Successfully!", ConsoleColor.Green);
        }

        private void EditTaskLog()
        {
            var activityLogs = this._activityLogService.GetAllActivityLogs();
            this.DisplayActivityLogs(activityLogs);
            TextColor.WriteColoredLine($"Enter index of task log to be edited: ", ConsoleColor.Cyan);
            int index;
            bool isValidIndex = InputValidator.ValidateInteger(Console.ReadLine(), out index);
            if (index < 1 || index > activityLogs.Count)
            {
                TextColor.WriteColoredLine($"Index Out of Range", ConsoleColor.Red);
                return;
            }

            index = index - 1;
            var activityLogToBeEdited = activityLogs[index];
            var input = this.GetTaskName();
            if (input == null)
            {
                return;
            }

            activityLogToBeEdited.LogName = input;
            this._activityLogService.EditTaskLog(activityLogToBeEdited);
            TextColor.WriteColoredLine($"Task log name edited successfully!", ConsoleColor.Green);
        }

        private void DisplayActivityLogs(List<ActivityLog> activityLogs)
        {
            for (int i = 0; i < activityLogs.Count; i++)
            {
                var activityLog = activityLogs[i];
                string endDisplay = activityLog.EndTime.HasValue ? activityLog.EndTime.ToString() : "Running";
                TimeSpan duration = activityLog.LogStatus == TimerStatus.Running ? (DateTime.Now - activityLog.StartTime) : activityLog.Duration;
                Console.WriteLine($"{i + 1}. {activityLog.LogName} Duration: {duration.ToString(@"hh\:mm\:ss")}");
                Console.WriteLine($"   {activityLog.StartTime} - {endDisplay}");
            }
        }

        private void EditTask()
        {
            var activities = this._activityService.GetAllActivities();
            this.DisplayActivities(activities);
            int index;
            bool isValidIndex = InputValidator.ValidateInteger(Console.ReadLine(), out index);
            if (index < 1 || index > activities.Count)
            {
                TextColor.WriteColoredLine($"Index Out of Range", ConsoleColor.Red);
                return;
            }

            index = index - 1;
            var input = this.GetTaskName();
            if (input == null)
            {
                return;
            }

            this._activityService.EditTaskName(activities[index].ActivityId, input);
            TextColor.WriteColoredLine($"Task name edited successfully!", ConsoleColor.Green);
        }

        private void Logout()
        {
            CurrentSessionManager.CurrentSessionUser = null;
            _userIO.DisplayMessage("Logging out", MessageType.Info);
        }

        private void AddNewTask()
        {
            if (!TryGetTaskName(out string taskName))
            {
                _userIO.DisplayMessage($"Invalid string format, Task name should not be null or empty!", MessageType.Error);
                return;
            }

            this._activityService.CreateNewTask(new Activity(taskName, TimeSpan.Zero, CurrentSessionManager.CurrentSessionUser.UserId));
            _userIO.DisplayMessage("Task Added Successfully!", MessageType.Success);
        }

        private bool TryGetTaskName(out string result)
        {
            _userIO.DisplayMessage("Enter Task Name: ");
            result = _userIO.ReadInput();
            if (!InputValidator.ValidateString(result))
            {
                return false;
        }

            return true;
        }

        private void ViewSpecificTaskLog()
        {
            HashSet<Activity> activities = this._activityService.GetAllActivities().ToHashSet();
            this.DisplayActivities(activities);

            _userIO.DisplayMessage($"Enter index of task to view activity details: ", MessageType.Info);
            if (!TryGetInputIndex(out int index))
            {
                _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
                return;
            }

            if (index < 1 || index > activities.Count)
            {
                _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
                return;
            }

            var activity = activities.ElementAt(index-1);
            var activityLogs = this._activityLogService.GetAllActivityLogs().Where(log => log.ActivityId == activity.ActivityId).OrderBy(a => a.StartTime);
            _userIO.DisplayMessage($"{activity.ActivityName}  Duration: {activity.TotalDuration.ToString(@"hh\:mm\:ss")}", MessageType.Info);
            this.DisplayLogDetails(activityLogs);
        }

        private void DisplayLogDetails(IOrderedEnumerable<ActivityLog> activityLogs)
        {
            var logsList = activityLogs.ToList();
            for (int i = 0; i < logsList.Count; i++)
            {
                var log = logsList[i];
                bool isRunning = log.LogStatus == TimerStatus.Running;
                string duration = isRunning ? string.Empty : $", Duration: {log.Duration:hh\\:mm\\:ss}";
                string endText = isRunning ? "Running" : log.EndTime?.ToString();
                MessageType messageType = isRunning ? MessageType.Success : MessageType.Default;
                _userIO.DisplayMessage(
                    $"{i + 1}. {log.LogName} {duration}\n" +
                    $"   Start: {log.StartTime},  End: {endText}",
                    messageType);
        }
        }


        private void StartTimer()
        {
            HashSet<Activity> activities = this.GetAllActivities().Where(a => a.TaskStatus != TimerStatus.Running).ToHashSet();
            if (activities.Count == 0)
            {
                _userIO.DisplayMessage($"No activities found to start Timer, first add new tasks", MessageType.Warning);
                return;
            }

            this.DisplayActivities(activities);
            _userIO.DisplayMessage($"Enter index of task to Start Timer: ", MessageType.Info);
            if (!TryGetInputIndex(out int index))
            {
                _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
                return;
            }

            if (index < 1 || index > activities.Count)
            {
                _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
                return;
            }

            index = index - 1;
            this._activityLogService.StartTimer(activities.ElementAt(index).ActivityId);
            _userIO.DisplayMessage($"Timer started Successfully!", MessageType.Success);
        }

        private IEnumerable<Activity> GetAllActivities()
        {
            return this._activityService.GetAllActivities();
        }

        private void DisplayActivities(HashSet<Activity> activities)
        {
            for (int i = 0; i < activities.Count; i++)
            {
                var activity = activities.ElementAt(i);
                _userIO.DisplayMessage($"{i + 1}. {activity.ActivityName}  Duration: {activity.TotalDuration.ToString(@"hh\:mm\:ss")}");
            }
        }

        private bool TryGetInputIndex(out int index)
        {
            return InputValidator.ValidateInteger(_userIO.ReadInput(), out index);
        }

        private void StopTimer()
        {
            HashSet<ActivityLog> activityLogs = this.GetAllActivityLogs().Where(a => a.LogStatus == TimerStatus.Running).ToHashSet();
            if (activityLogs.Count == 0)
            {
                _userIO.DisplayMessage($"No Active timers to stop", MessageType.Warning);
                return;
            }

            this.DisplayActivityLogs(activityLogs);
            _userIO.DisplayMessage($"Enter index of task to Stop Timer: ", MessageType.Info);
            if (!TryGetInputIndex(out int index))
            {
                _userIO.DisplayMessage("Invalid integer format", MessageType.Error);
                return;
            }

            if (index < 1 || index > activityLogs.Count)
            {
                _userIO.DisplayMessage($"Index Out of Range", MessageType.Error);
                return;
            }

            index = index - 1;
            this._activityLogService.StopTimer(activityLogs.ElementAt(index).ActivityLogId);
            _userIO.DisplayMessage($"Timer stopped successfully!", MessageType.Success);
        }

        private void DisplayActivityLogs(HashSet<ActivityLog> activityLogs)
        {

            for (int i = 0; i < activityLogs.Count; i++)
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
            throw new NotImplementedException();
        }

        private void ExportTimeLog()
        {
            throw new NotImplementedException();
        }
    }
}
