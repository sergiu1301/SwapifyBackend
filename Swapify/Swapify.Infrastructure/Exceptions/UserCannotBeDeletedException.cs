namespace Swapify.Infrastructure.Exceptions;

public class UserCannotBeDeletedException : BaseException
{
    public UserCannotBeDeletedException() : base(ErrorMessages.UserCannotBeDeleted)
    {
    }

    public override string ErrorCode => ErrorCodes.UserCannotBeDeleted;
    public override ErrorTypes ErrorType => ErrorTypes.Conflict;
}