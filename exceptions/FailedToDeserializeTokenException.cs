public class FailedToDeserializeTokenException : Exception
{
    public FailedToDeserializeTokenException() { }
    public FailedToDeserializeTokenException(string message) : base(message) { }
    public FailedToDeserializeTokenException(string message, Exception inner) : base(message, inner) { }
}