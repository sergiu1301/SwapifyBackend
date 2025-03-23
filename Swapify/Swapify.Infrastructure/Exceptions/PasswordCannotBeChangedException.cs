namespace Swapify.Infrastructure.Exceptions;

public class PasswordCannotBeChangedException : BaseException
{
    public PasswordCannotBeChangedException() : base(ErrorMessages.PasswordCannotBeChanged)
    {
    }

    public override string ErrorCode => ErrorCodes.PasswordCannotBeChanged;
    public override ErrorTypes ErrorType => ErrorTypes.Conflict;
}