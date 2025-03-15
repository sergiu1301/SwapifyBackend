namespace Swapify.Infrastructure.Exceptions;

public class RoleNotFoundException : BaseException
{
    public RoleNotFoundException() : base(ErrorMessages.RoleNotFound)
    {
    }

    public override string ErrorCode => ErrorCodes.RoleNotFound;
    public override ErrorTypes ErrorType => ErrorTypes.ResourceNotFound;
}