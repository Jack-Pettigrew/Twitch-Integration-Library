using System.Net;

namespace TIL.Network.Responses;

public record TokenFailed
{
    public HttpStatusCode status { get; init; }
    public string? message { get; init; }
}
