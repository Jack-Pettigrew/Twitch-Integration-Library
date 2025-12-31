namespace TIL.Exceptions;

public class DeviceCodeExpiredExceptionException : System.Exception
{
    public DeviceCodeExpiredExceptionException() { }
    public DeviceCodeExpiredExceptionException(string message) : base(message) { }
    public DeviceCodeExpiredExceptionException(string message, System.Exception inner) : base(message, inner) { }
}