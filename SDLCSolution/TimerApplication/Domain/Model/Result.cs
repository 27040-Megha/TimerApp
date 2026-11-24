namespace TimerApplication.Domain.Model; 

public class Result
{
    public bool IsSuccess { get; }
    public string Message { get; }

    public Result(bool isSuccess, string message)
    {
        this.IsSuccess = isSuccess;
        this.Message = message;
    }
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
