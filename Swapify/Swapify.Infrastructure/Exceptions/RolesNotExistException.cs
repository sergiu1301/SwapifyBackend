namespace Swapify.Infrastructure.Exceptions;

public class RolesNotExistException : BaseException
{
    public RolesNotExistException() : base(ErrorMessages.RolesNotExist)
    {
    }

    public override string ErrorCode => ErrorCodes.RolesNotExist;
    public override ErrorTypes ErrorType => ErrorTypes.ResourceNotFound;
}