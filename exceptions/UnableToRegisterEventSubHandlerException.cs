public class UnableToRegisterEventSubHandlerException : Exception
{
    public UnableToRegisterEventSubHandlerException() { }
    public UnableToRegisterEventSubHandlerException(string message) : base(message) { }
    public UnableToRegisterEventSubHandlerException(string message, Exception inner) : base(message, inner) { }
}