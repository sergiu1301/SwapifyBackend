namespace Swapify.Infrastructure.Exceptions;

public abstract class BaseException : Exception
{
    public override string Message { get; }

    public abstract string ErrorCode { get; }

    public abstract ErrorTypes ErrorType { get; }

    protected BaseException(string message) : base(message)
    {
        Message = message;
    }
}