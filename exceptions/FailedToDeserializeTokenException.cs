public class FailedToDeserializeTokenException : System.Exception
{
    public FailedToDeserializeTokenException() { }
    public FailedToDeserializeTokenException(string message) : base(message) { }
    public FailedToDeserializeTokenException(string message, System.Exception inner) : base(message, inner) { }
}