namespace Comforix.Shared;

public class MessagePacket
{
    public string Token { get; set; } = string.Empty;
    public string Dest { get; set; } = string.Empty;
    public string DestUsername { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}