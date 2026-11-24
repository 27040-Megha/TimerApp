using System.Linq;
using System.Text.RegularExpressions;
using TimerApplication.Domain.Model;

namespace TimerApplication.PresentationLayer.Helper;

public static class InputValidator
{
    public static bool ValidateInteger(string? input, out int number)
    {
        return int.TryParse(input, out number);
    }

    public static bool ValidateEmail(string? email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return false;
        }

        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email.Trim(), pattern);
    }

    public static Result ValidatePassword(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
        {
            return new Result(false, $"The password should atleast be of length 8");
        }

        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));
        if (!(hasUpper && hasLower && hasSpecial))
        {
            return new Result(false, $"The password should contain atleast 1 Uppercase, 1 Lowercase and 1 Special Character");
        }

        return new Result(true, $"Valid Password");
    }

    public static bool ValidateString(string input)
    {
        return !string.IsNullOrEmpty(input);
    }
}
