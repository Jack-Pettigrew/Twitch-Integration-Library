using System;

namespace TIL.Events;

public class BitsUse : EventArgs
{
    public string UserId { init; get; } = "";
    public string UserName { init; get; } = "";
    public int Bits { init; get; } = 0;
    public string Type { init; get; } = "";
    public string Message { init; get; } = "";
}