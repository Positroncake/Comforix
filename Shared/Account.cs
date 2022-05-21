using Comforix.Shared;

namespace ayayay.Shared;

public class Account
{
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>(); // 64 Bytes (512-bit)
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>(); // 128 Bytes (1024-bit)
    public PhysicalOrEmotional Level1 { get; set; } // Physical or emotional issue
    public Issue Level2 { get; set; } // Specific issue
    public string? Level3 { get; set; } // More details
    public int Reputation { get; set; }
}