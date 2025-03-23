namespace Swapify.Infrastructure.Exceptions;

public class EmailAlreadyConfirmedException : BaseException
{
    public EmailAlreadyConfirmedException() : base(ErrorMessages.EmailAlreadyConfirmed)
    {
    }

    public override string ErrorCode => ErrorCodes.EmailAlreadyConfirmed;
    public override ErrorTypes ErrorType => ErrorTypes.ResourceAlreadyExists;
}