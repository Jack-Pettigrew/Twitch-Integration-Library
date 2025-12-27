namespace TIL.Exceptions;

public class InvalidAccessTokenException : System.Exception
{
    public InvalidAccessTokenException() { }
    public InvalidAccessTokenException(string message) : base(message) { }
    public InvalidAccessTokenException(string message, System.Exception inner) : base(message, inner) { }
    protected InvalidAccessTokenException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}