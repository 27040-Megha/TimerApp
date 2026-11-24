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
public class Result<T>
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public List<T> Data { get; }

    public Result(bool isSuccess, string message)
    {
        this.IsSuccess = isSuccess;
        this.Message = message;
        this.Data = [];
    }

    public Result(bool isSuccess, string message, List<T> data)
        : this(isSuccess, message)
    {
        this.Data = data;
    }
}
