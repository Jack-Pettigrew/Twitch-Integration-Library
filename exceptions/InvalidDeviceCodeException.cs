namespace TIL.Exceptions;

public class InvalidDeviceCodeException : System.Exception
{
    public InvalidDeviceCodeException() { }
    public InvalidDeviceCodeException(string message) : base(message) { }
    public InvalidDeviceCodeException(string message, System.Exception inner) : base(message, inner) { }
}