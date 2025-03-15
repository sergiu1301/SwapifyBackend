namespace Swapify.Infrastructure.Exceptions;

public class RoleAlreadyExistsException : BaseException
{
    public RoleAlreadyExistsException() : base(ErrorMessages.RoleAlreadyExists)
    {
    }

    public override string ErrorCode => ErrorCodes.RoleAlreadyExists;
    public override ErrorTypes ErrorType => ErrorTypes.ResourceAlreadyExists;
}