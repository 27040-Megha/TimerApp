using System;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.PresentationLayer.Helper;

namespace TimerApplication.PresentationLayer.View
{
    public class TaskDashboard
    {
        private readonly UserService _userService;

        public TaskDashboard(UserService userService)
        {
            this._userService = userService;
        }

        public void RunDashboard()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            Console.Clear();
            TextColor.WriteColoredLine($"Welcome {CurrentSessionManager.CurrentSessionUser.UserName}", ConsoleColor.Blue);
            int choice;
            do
            {
                Console.WriteLine("1. Add new Task");
                Console.WriteLine("2. Edit Task");
                Console.WriteLine("3. Delete Task");
                Console.WriteLine("4. View Specific Task Log");
                Console.WriteLine("5. Start Timer");
                Console.WriteLine("6. Stop Timer");
                Console.WriteLine("7. Import Time Log");
                Console.WriteLine("8. Export Time Log");
                Console.WriteLine("9. Logout");
                Console.WriteLine("Enter Your Option (1 to 9): ");
                bool isValidChoice = InputValidator.ValidateInteger(Console.ReadLine(), out choice);
                if (!isValidChoice)
                {
                    TextColor.WriteColoredLine($"Invalid Integer format, Enter a valid integer!", ConsoleColor.Red);
                    continue;
                }

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
                        TextColor.WriteColoredLine("Invalid Choice, Enter a valid choice between 1 to 9", ConsoleColor.Red);
                        break;
                }
            }
            while (choice != 9);
        }

        private void Logout()
        {
            CurrentSessionManager.CurrentSessionUser = null;
            this._userService.SaveCurrentActiveUser();
            TextColor.WriteColoredLine("Logging out", ConsoleColor.Yellow);
        }

        private void AddNewTask()
        {
            throw new NotImplementedException();
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
