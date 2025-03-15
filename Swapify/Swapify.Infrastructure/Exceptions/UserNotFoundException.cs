namespace Swapify.Infrastructure.Exceptions;

public class UserNotFoundException : BaseException
{
    public UserNotFoundException() : base(ErrorMessages.UserNotFound)
    {
    }

    public override string ErrorCode => ErrorCodes.UserNotFound;
    public override ErrorTypes ErrorType => ErrorTypes.ResourceNotFound;
}