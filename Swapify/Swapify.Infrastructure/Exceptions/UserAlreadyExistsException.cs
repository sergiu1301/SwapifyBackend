namespace Swapify.Infrastructure.Exceptions;

public class UserAlreadyExistsException : BaseException
{
    public UserAlreadyExistsException() : base(ErrorMessages.UserAlreadyExists)
    {
    }

    public override string ErrorCode => ErrorCodes.UserAlreadyExists;
    public override ErrorTypes ErrorType => ErrorTypes.ResourceAlreadyExists;
}