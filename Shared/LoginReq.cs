using System;

namespace Comforix.Shared;

public class LoginReq
{
    public string Username { get; set; } = string.Empty;
    public byte[] Password { get; set; } = Array.Empty<byte>(); // 64 Bytes (512-bit)
}