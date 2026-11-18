using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TimerApplication.ApplicationLayer.Domain.Enums;
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

    private readonly IUserIO _userIO;

    private readonly TaskDashboard _taskDashboard;

    private delegate bool ValidateInput(string input, out string errorMessage);

    private readonly ActivityService _activityService;

    public ViewOperator(UserService userService, IUserIO userIO, TaskDashboard taskDashboard, ActivityService activityService)
    {
        this._userService = userService;
        this._activityService = activityService;
        this._userIO = userIO;
        this._taskDashboard = taskDashboard;
    }

    public void Run()
    {
        LoadPreviousUserSession();
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

    public void DisplayCurrentUsersTask()
    {
        if (CurrentSessionManager.CurrentSessionUser != null)
        {
            int left = 55;
            int width = _userIO.WindowWidth - left - 1;
            if (width <= 10)
            {
                return;
            }

            _userIO.DisplayMessageAt(left, 0, "Your activities: ", MessageType.Info);
            var activities = this.GetAllActivities();
            for (int i = 0; i < activities.Count; i++)
            {
                var activity = activities.ElementAt(i);
                string text = $"{i + 1}. {activity.ActivityName}  Duration: {activity.TotalDuration}";
                _userIO.DisplayMessageAt(left, i + 1, text.PadRight(width));
            }

            _userIO.DisplayMessageAt(left, activities.Count + 1, new string(' ', width));
        }
    }

    private HashSet<Activity> GetAllActivities()
    {
        return this._activityService.GetAllActivities().ToHashSet();
    }

    private void LoadPreviousUserSession()
    {
        User activeUser = _userService.FetchActiveUser();
        if (activeUser != null)
        {
            CurrentSessionManager.CurrentSessionUser = activeUser;
            _userIO.DisplayMessage($"Welcome back {CurrentSessionManager.CurrentSessionUser.UserName}!", MessageType.Info);
            _taskDashboard.RunDashboard();
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
        _userIO.DisplayMessage("1. Signup \n2. Login \n3. Exit \nEnter Your Option (1/2/3): ");
    }

    private void LoginUser()
    {
        if (!TryGetInput("Email", _userIO.ReadInput, ValidateEmail, out string email) ||
            !TryGetInput("Password", _userIO.ReadMaskedPassword, ValidatePassword, out string password))
        {
            _userIO.DisplayMessage("Login failed: Due to many failed attempts", MessageType.Warning);
            return;
        }

        Result authenticationStatus = this._userService.LoginUser(email, password);
        if (authenticationStatus.IsSuccess)
        {
            _userIO.DisplayMessage("Login Successful", MessageType.Success);
            _taskDashboard.RunDashboard();
        }
        else
        {
            if (authenticationStatus.LoginStatus == AuthenticationStatus.AccountNotExists)
            {
                _userIO.DisplayMessage("Account doesn't exist, Signup first", MessageType.Warning);
                SignupUser();
            }
            else
            {
                _userIO.DisplayMessage("Invalid credentials, Try logging in again", MessageType.Error);
                return;
            }
        }
    }

    private void SignupUser()
    {
        if (!TryGetInput("User name",_userIO.ReadInput, ValidateName, out string userName) ||
            !TryGetInput("Email", _userIO.ReadInput, ValidateEmail, out string email) ||
            !TryGetInput("Password", _userIO.ReadMaskedPassword, ValidatePassword, out string password))
        {
            _userIO.DisplayMessage("Signup failed: Due to many failed attempts", MessageType.Warning);
            return;
        }

        string hashedPassword = PasswordHasher.HashPassword(password);
        User newUser = new User(userName, email, hashedPassword);
        if (!_userService.CreateUser(newUser))
        {
            _userIO.DisplayMessage("Signup failed: Account already registered with same email ID", MessageType.Error);
            return;
        }

        _userIO.DisplayMessage("Signup Successful!", MessageType.Success);
    }

    private bool TryGetInput(string fieldName, Func<string> readInput, ValidateInput validateInput, out string result)
    {
        result = null;
        _userIO.DisplayMessage($"Enter {fieldName}: ");
        for (int attempt = 1; attempt <= MaxInputAttempts; attempt++)
        {
            string input = readInput();
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

    private static bool ValidateName(string input, out string error)
    {
        error = "User name cannot be empty!";
        return InputValidator.ValidateString(input);
    }

    private static bool ValidateEmail(string input, out string error)
    {
        error = "Invalid email format!";
        return InputValidator.ValidateEmail(input);
    }


    private static bool ValidatePassword(string input, out string error)
    {
        var passwordCheckResult = InputValidator.ValidatePassword(input);
        error = passwordCheckResult.Message;
        return passwordCheckResult.IsSuccess;
    }
}
