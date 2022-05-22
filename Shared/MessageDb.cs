namespace Comforix.Shared;

public class MessageDb
{
    public string Username { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool Sending { get; set; }
    public DateTime SentTime { get; set; }
}