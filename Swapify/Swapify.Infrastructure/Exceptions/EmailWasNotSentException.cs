namespace Swapify.Infrastructure.Exceptions;

public class EmailWasNotSentException : BaseException
{
    public EmailWasNotSentException() : base(ErrorMessages.EmailWasNotSent)
    {
    }

    public override string ErrorCode => ErrorCodes.EmailWasNotSent;
    public override ErrorTypes ErrorType => ErrorTypes.FailedDependency;
}