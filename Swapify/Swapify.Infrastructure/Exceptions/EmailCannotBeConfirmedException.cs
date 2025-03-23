namespace Swapify.Infrastructure.Exceptions;

public class EmailCannotBeConfirmedException : BaseException
{
    public EmailCannotBeConfirmedException() : base(ErrorMessages.EmailCannotBeConfirmed)
    {
    }

    public override string ErrorCode => ErrorCodes.EmailCannotBeConfirmed;
    public override ErrorTypes ErrorType => ErrorTypes.Conflict;
}