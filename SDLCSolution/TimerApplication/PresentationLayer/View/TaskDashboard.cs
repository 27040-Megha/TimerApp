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
                    _userIO.DisplayMessage("Invalid Choice, Enter a valid choice between 1 to 3", MessageType.Error);
                        break;
                }
            }
            while (choice != 9);
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

        private void EditTask()
        {
            throw new NotImplementedException();
        }

        private void DeleteTask()
        {
            throw new NotImplementedException();
        }

        private void ViewSpecificTaskLog()
        {
            var activities = this._activityService.GetAllActivities();
            for (int i = 0; i < activities.Count; i++)
            {
                var task = activities[i];
                Console.WriteLine($"{i + 1}. {task.ActivityName}  Duration: {task.TotalDuration.ToString(@"hh\:mm\:ss")}");
            }

            TextColor.WriteColoredLine($"Enter index of task to Start Timer: ", ConsoleColor.Cyan);
            int index;
            bool isValidIndex = InputValidator.ValidateInteger(Console.ReadLine(), out index);
            if (index < 1 || index > activities.Count)
            {
                TextColor.WriteColoredLine($"Index Out of Range", ConsoleColor.Red);
                return;
            }

            index = index - 1;
            var activity = activities[index];
            var activityLogs = this._activityLogService.GetAllActivityLogs().Where(a => a.TaskId == activity.ActivityId).OrderBy(a => a.StartTime);
            TextColor.WriteColoredLine($"{activity.ActivityName}  Duration: {activity.TotalDuration.ToString(@"hh\:mm\:ss")}", ConsoleColor.DarkCyan);
            foreach (var log in activityLogs)
            {
                Console.WriteLine($"{log.LogName},  Start: {log.StartTime},  End: {log.EndTime}, Duration: {log.Duration.ToString(@"hh\:mm\:ss")}");
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
