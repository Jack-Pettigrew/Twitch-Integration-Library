namespace TIL.Exceptions;

public class DeviceCodeExpiredExceptionException : Exception
{
    public DeviceCodeExpiredExceptionException() { }
    public DeviceCodeExpiredExceptionException(string message) : base(message) { }
    public DeviceCodeExpiredExceptionException(string message, Exception inner) : base(message, inner) { }
}