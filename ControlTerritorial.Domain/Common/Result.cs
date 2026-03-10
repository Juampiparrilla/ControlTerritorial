namespace ControlTerritorial.Domain.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public bool IsFail => !IsSuccess;
        public string? ErrorMessage { get; private set; }
        public T? Value { get; private set; }

        public Result(bool isSuccess, T value, string? errorMessage)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorMessage = errorMessage;
        }
        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, string.Empty);
        }   

        public static Result<T> Fail(string errorMessage)
        {
            return new Result<T>(false, default!, errorMessage);
        }

    }
}
