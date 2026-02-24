public class TwitchEventSubExistsException : System.Exception
{
    public TwitchEventSubExistsException() { }
    public TwitchEventSubExistsException(string message) : base(message) { }
    public TwitchEventSubExistsException(string message, System.Exception inner) : base(message, inner) { }
}