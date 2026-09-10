namespace SFM_BE.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string message, string errorCode)
        : base(message, 403, errorCode)
    {
    }
}
