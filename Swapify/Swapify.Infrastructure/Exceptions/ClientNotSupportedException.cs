namespace Swapify.Infrastructure.Exceptions;

public class ClientNotSupportedException : BaseException
{
    public ClientNotSupportedException() : base(ErrorMessages.ClientNotSupported)
    {
    }

    public override string ErrorCode => ErrorCodes.ClientNotSupported;
    public override ErrorTypes ErrorType => ErrorTypes.ClientError;
}