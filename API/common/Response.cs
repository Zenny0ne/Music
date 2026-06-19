namespace API.Common;

public class Response<T>
{
    public bool IsSuccess { get; }
    public T Data { get; }
    public string? Error { get; }
    public string? Message { get; set; }
    public Response(bool isSuccess, T Data, string? Error, string? Message)
    {
        this.IsSuccess = isSuccess;
        this.Data = Data;
        this.Error = Error;
        this.Message = Message;
    }
    public static Response<T> Success(T data, string? Message = "") => new (true, data, null, Message);
    public static Response<T> Failure(string error) => new (false, default!, error, null);
}
