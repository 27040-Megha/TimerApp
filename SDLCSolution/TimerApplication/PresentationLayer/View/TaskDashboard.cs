using System;
using TimerApplication.ApplicationLayer.Domain.Enums;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.PresentationLayer.Helper;
using TimerApplication.Domain.Model;
using System.Collections.Generic;

namespace TimerApplication.PresentationLayer.View
{
    public class TaskDashboard
    {
        private readonly UserService _userService;

        private readonly IUserIO _userIO;

        private readonly ActivityService _activityService;

        public TaskDashboard(UserService userService, ActivityService activityService, IUserIO userIO)
        {
            this._userService = userService;
            this._activityService = activityService;
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

        private void Logout()
        {
            CurrentSessionManager.CurrentSessionUser = null;
            _userIO.DisplayMessage("Logging out", MessageType.Info);
        }

        private void AddNewTask()
        {
            if (!TryGetTaskName(out string taskName))
            {
                _userIO.DisplayMessage($"Invalid strig format, Task name should not be null or empty!", MessageType.Error);
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
            throw new NotImplementedException();
        }

        private void StartTimer()
        {
            throw new NotImplementedException();
        }

        private void StopTimer()
        {
            throw new NotImplementedException();
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
