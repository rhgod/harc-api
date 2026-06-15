public class BusinessException : Exception
{
    public string ErrorCode { get; }

    public BusinessException(string errorCode) : base()
    {
        ErrorCode = errorCode;
    }
}