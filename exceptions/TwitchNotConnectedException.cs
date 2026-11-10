namespace TIL.Exceptions;

public class TwitchNotConnectedException : Exception
{
    public TwitchNotConnectedException() { }
    public TwitchNotConnectedException(string message) : base(message) { }
    public TwitchNotConnectedException(string message, Exception inner) : base(message, inner) { }
}