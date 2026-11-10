public class TwitchEventSubExistsException : Exception
{
    public TwitchEventSubExistsException() { }
    public TwitchEventSubExistsException(string message) : base(message) { }
    public TwitchEventSubExistsException(string message, Exception inner) : base(message, inner) { }
}