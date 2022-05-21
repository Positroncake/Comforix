using System.Security.Cryptography;
using DatabaseAccess;

namespace Comforix.Server;

public static class TokenUtils
{
    public static async Task<string> NewToken(string username)
    {
        // Generate token
        var bytes = new byte[64];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        string token = Convert.ToHexString(bytes).ToLowerInvariant();

        // Store into database
        IAccess access = new Access();
        const string sql = "INSERT INTO tokens (Token, Username) VALUES (@Token, @Username)";
        await access.ExecuteAsync(sql, new
        {
            Token = token,
            Username = username
        });

        return token;
    }
}