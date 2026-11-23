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

    public Result(bool isSuccess, ErrorCode errorCode)
    {
        this.IsSuccess = isSuccess;
        this.ErrorType = errorCode;
    }

    public bool IsSuccess { get; set; }

    public string Message { get; set; }

    public ErrorCode ErrorType { get; set; }

    public AuthenticationStatus LoginStatus { get; set; }
}
