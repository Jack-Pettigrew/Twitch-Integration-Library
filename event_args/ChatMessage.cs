namespace TIL.Events;

public class ChatMessage : EventArgs
{
    public string MessageId { set; get; } = "";
    public string UserId { set; get; }
    public string UserName { set; get; } = "";
    public string Message { set; get; } = "";
    public bool IsReply { set; get; } = false;
}