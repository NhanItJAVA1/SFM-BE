namespace SFM_BE.Exceptions;

public class ConflictException : AppException
{
    public ConflictException(string message, string errorCode)
        : base(message, 409, errorCode)
    {
    }
}
