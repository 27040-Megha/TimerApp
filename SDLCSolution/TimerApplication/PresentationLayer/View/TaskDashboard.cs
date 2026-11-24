using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.Domain.Enums;
using TimerApplication.PresentationLayer.Helper;

namespace TimerApplication.PresentationLayer.View;

public class TaskDashboard
{
    private readonly UserService _userService;

    private readonly ISessionManager _currentSessionManager;

    private readonly IUserIO _userIO;

    public TaskDashboard(UserService userService, IUserIO userIO, ISessionManager currentSessionManager)
    {
        this._userService = userService;
        this._userIO = userIO;
        this._currentSessionManager = currentSessionManager;
    }

    public void RunDashboard()
    {
        _userIO.ClearOutput();
        _userIO.DisplayMessage($"Welcome {_currentSessionManager.CurrentSessionUser?.UserName}", MessageType.Info);
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
        _userIO.DisplayMessage("Not available yet", MessageType.Warning);
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