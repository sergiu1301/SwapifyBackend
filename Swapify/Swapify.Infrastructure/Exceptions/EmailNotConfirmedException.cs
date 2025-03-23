namespace Swapify.Infrastructure.Exceptions;

public class EmailNotConfirmedException : BaseException
{
    public EmailNotConfirmedException() : base(ErrorMessages.EmailNotConfirmed)
    {
    }

    public override string ErrorCode => ErrorCodes.EmailNotConfirmed;
    public override ErrorTypes ErrorType => ErrorTypes.AccessForbidden;
}