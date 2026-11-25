using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.Domain.Enums;
using TimerApplication.Domain.Model;
using TimerApplication.PresentationLayer.Helper;

namespace TimerApplication.PresentationLayer.View;

public class ViewOperator
{
    private const int MaxInputAttempts = 3;

    private readonly UserService _userService;

    private readonly ISessionManager _currentSessionManager;

    private readonly IUserIO _userIO;

    private readonly TaskDashboard _taskDashboard;

    private delegate bool ValidateInput(string? input, out string errorMessage);

    private readonly ActivityService _activityService;

    private readonly ActivityLogService _activityLogService;

    public ViewOperator(UserService userService, IUserIO userIO, TaskDashboard taskDashboard, ActivityService activityService, ActivityLogService activityLogService, ISessionManager currentSessionManager)
    {
        this._userService = userService;
        this._userIO = userIO;
        this._activityService = activityService;
        this._taskDashboard = taskDashboard;
        this._activityLogService = activityLogService;
        this._currentSessionManager = currentSessionManager;
    }

    public void Run()
    {
        try
        {
            LoadPreviousUserSession();
        }
        catch (Exception ex) when (ex is InvalidDataException or IOException or UnauthorizedAccessException)
        {
            _userService.ClearSession();       
        }

        _userIO.DisplayMessage("Welcome to Time Logger", MessageType.Info);
        int choice;
        do
        {
            this.DisplayMenu();
            if (!InputValidator.ValidateInteger(_userIO.ReadInput(), out choice))
            {
                _userIO.DisplayMessage("Invalid Integer format, Enter a valid integer!", MessageType.Error);
                continue;
            }
            try
            {
                this.HandleMenuChoice(choice);
            }
            catch (Exception ex) when (ex is InvalidDataException or IOException or UnauthorizedAccessException)
            {
                _userIO.DisplayMessage($"Exception occurred during file Handling - {ex.Message}", MessageType.Error);
            }
            catch (Exception ex)
            {
                _userIO.DisplayMessage($"An unexpected error occurred: {ex.Message}", MessageType.Error);
            }
        }
        while (choice != 3);
    }

    public void SaveCurrentActiveUser()
    {
        this._userService.SaveCurrentActiveUser();
    }

    private void LoadPreviousUserSession()
    {
        Guid? activeUserId = _userService.FetchActiveUser();
        if (activeUserId == null)
        {
            return;
        }

        User? activeUser = _userService.FindUserById(activeUserId);
        if (activeUser != null)
        {
            _currentSessionManager.CurrentSessionUser = activeUser;
            _userIO.DisplayMessage($"Welcome back {activeUser.UserName}!", MessageType.Info);
            _taskDashboard.RunDashboard();
        }
        else
        {
            _userService.ClearSession();
        }
    }

    private void HandleMenuChoice(int choice)
    {
        switch (choice)
        {
            case 1:
                this.SignupUser();
                break;
            case 2:
                this.LoginUser();
                break;
            case 3:
                _userIO.DisplayMessage("Exiting the App, Bye!", MessageType.Info);
                break;
            default:
                _userIO.DisplayMessage("Invalid Choice, Enter a valid choice between 1 to 3", MessageType.Error);
                break;
        }
    }

    private void DisplayMenu()
    {
        _userIO.DisplayMessage("Welcome to Time Logger", MessageType.Info);
        _userIO.DisplayMessageAtSameLine("1. Signup \n2. Login \n3. Exit \nEnter Your Option (1/2/3): ");
    }

    private void LoginUser()
    {
        if (!TryGetInput("Email", _userIO.ReadInput, ValidateEmail, out string? email) ||
            !TryGetInput("Password", _userIO.ReadMaskedPassword, ValidatePassword, out string? password))
        {
            _userIO.DisplayMessage("Login failed: Due to many failed attempts", MessageType.Warning);
            return;
        }

        AuthenticationStatus authenticationStatus = this._userService.LoginUser(email, password);
        switch(authenticationStatus)
        {
            case AuthenticationStatus.AccountNotExists:
                _userIO.DisplayMessage("Account doesn't exist, Signup first", MessageType.Error);
                break;
            case AuthenticationStatus.InvalidPassword:
                _userIO.DisplayMessage("Invalid credentials, Try logging in again", MessageType.Error);
                break;
            case AuthenticationStatus.Successful:
                _userIO.DisplayMessage("Login Successful", MessageType.Success);
                _taskDashboard.RunDashboard();
                break;
        }
    }

    private void SignupUser()
    {
        if (!TryGetInput("User name",_userIO.ReadInput, ValidateName, out string? userName) ||
            !TryGetInput("Email", _userIO.ReadInput, ValidateEmail, out string? email) ||
            !TryGetInput("Password", _userIO.ReadMaskedPassword, ValidatePassword, out string? password))
        {
            _userIO.DisplayMessage("Signup failed: Due to many failed attempts", MessageType.Warning);
            return;
        }

        string? hashedPassword = PasswordHasher.HashPassword(password);
        User? newUser = new User(userName, email, hashedPassword);
        if (!_userService.CreateUser(newUser))
        {
            _userIO.DisplayMessage("Signup failed: Account already registered with same email ID", MessageType.Error);
            return;
        }

        _userIO.DisplayMessage("Signup Successful!", MessageType.Success);
    }

    private bool TryGetInput(string fieldName, Func<string?> readInput, ValidateInput validateInput, out string? result)
    {
        result = null;
        _userIO.DisplayMessage($"Enter {fieldName}: ");
        for (int attempt = 1; attempt <= MaxInputAttempts; attempt++)
        {
            string? input = readInput()?.Trim();
            if (validateInput(input, out string error))
            {
                result = input;
                return true;
            }

            _userIO.DisplayMessage(error, MessageType.Error);
            if (attempt < MaxInputAttempts)
            {
                _userIO.DisplayMessage($"Re-enter {fieldName}: ");
            }
        }

        return false;
    }

    private static bool ValidateName(string? input, out string error)
    {
        error = "User name cannot be empty!";
        return InputValidator.ValidateString(input);
    }

    private static bool ValidateEmail(string? input, out string error)
    {
        error = "Invalid email format!";
        return InputValidator.ValidateEmail(input);
    }


    private static bool ValidatePassword(string? input, out string error)
    {
        Result passwordCheckResult = InputValidator.ValidatePassword(input);
        error = passwordCheckResult.Message!;
        return passwordCheckResult.IsSuccess;
    }
}
