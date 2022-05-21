namespace ayayay.Shared;

public class AccountBase
{
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>(); // 64 Bytes (512-bit)
    public byte[]? PasswordSalt { get; set; }
    // 128 Bytes (1024-bit)
    // Null in case of LOGIN (salt is read from database instead of user)
    // Not null in case of REGISTER (salt is generated client-side and then sent to server)
}