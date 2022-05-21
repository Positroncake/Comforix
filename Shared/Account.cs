using System;

namespace Comforix.Shared;

public class Account
{
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>(); // 64 Bytes (512-bit)
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>(); // 128 Bytes (1024-bit)
    public byte Level1 { get; set; } // Physical issue, emotional issue, or both
    public string Level2 { get; set; } = string.Empty; // Specific issues
    public string? Level3 { get; set; } // More details
    public int Reputation { get; set; }
}