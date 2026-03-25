namespace VoteAndQuizWebApi.Services
{
    public enum ServiceErrorType
    {
        Validation,
        NotFound,
        Unauthorized,
        Conflict,
        Failure
    }

    public class ServiceResult
    {
        public bool Succeeded { get; init; }
        public string? ErrorMessage { get; init; }
        public ServiceErrorType? ErrorType { get; init; }

        public static ServiceResult Success() => new ServiceResult { Succeeded = true };

        public static ServiceResult Fail(ServiceErrorType errorType, string errorMessage) =>
            new ServiceResult
            {
                Succeeded = false,
                ErrorType = errorType,
                ErrorMessage = errorMessage
            };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; init; }

        public static ServiceResult<T> Success(T data) =>
            new ServiceResult<T>
            {
                Succeeded = true,
                Data = data
            };

        public new static ServiceResult<T> Fail(ServiceErrorType errorType, string errorMessage) =>
            new ServiceResult<T>
            {
                Succeeded = false,
                ErrorType = errorType,
                ErrorMessage = errorMessage
            };
    }
}
