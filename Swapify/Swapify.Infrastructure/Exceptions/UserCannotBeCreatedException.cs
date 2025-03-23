namespace Swapify.Infrastructure.Exceptions;

public class UserCannotBeCreatedException : BaseException
{
    public UserCannotBeCreatedException() : base(ErrorMessages.UserCannotBeCreated)
    {
    }

    public override string ErrorCode => ErrorCodes.UserCannotBeCreated;
    public override ErrorTypes ErrorType => ErrorTypes.Conflict;
}