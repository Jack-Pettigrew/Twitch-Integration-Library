namespace TIL.Exceptions;

public class InvalidDeviceCodeException : Exception
{
    public InvalidDeviceCodeException() { }
    public InvalidDeviceCodeException(string message) : base(message) { }
    public InvalidDeviceCodeException(string message, Exception inner) : base(message, inner) { }
}