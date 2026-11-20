using TimerApplication.Domain.Enums;

namespace TimerApplication.Domain.Model;

public class Result
{
    public Result(bool isSuccess, string message)
    {
        this.IsSuccess = isSuccess;
        this.Message = message;
    }

    public Result(bool isSuccess, AuthenticationStatus authenticationStatus)
    {
        this.IsSuccess = isSuccess;
        this.LoginStatus = authenticationStatus;
    }

    public bool IsSuccess { get; set; }

    public string Message { get; set; }

    public AuthenticationStatus LoginStatus { get; set; }
}
