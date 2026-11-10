namespace TIL.Exceptions;

public class MissingScopesException : Exception
{
    public MissingScopesException() { }
    public MissingScopesException(string message) : base(message) { }
    public MissingScopesException(string message, Exception inner) : base(message, inner) { }
}