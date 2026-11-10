namespace TIL.DataContainers;

public sealed record Pagination
{
    public string? Cursor { get; set; }
}