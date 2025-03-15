namespace Swapify.Infrastructure.Exceptions;

public class UserNameOrPasswordNotFoundException : BaseException
{
    public UserNameOrPasswordNotFoundException() : base(ErrorMessages.UserNameOrPasswordNotFoundException)
    {
    }

    public override string ErrorCode => ErrorCodes.UserNameOrPasswordNotFound;
    public override ErrorTypes ErrorType => ErrorTypes.ResourceNotFound;
}