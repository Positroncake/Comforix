using System.Security.Cryptography;
using System.Text;
using Comforix.Shared;
using DatabaseAccess;

namespace Comforix.Server;

public static class Utils
{
    public static async Task<string> NewToken(string username)
    {
        // Generate token
        var bytes = new byte[64];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        string token = Convert.ToHexString(bytes).ToLowerInvariant();

        // Store into database
        IAccess database = new Access();
        const string sql = "INSERT INTO tokens (Token, Username) VALUES (@Token, @Username)";
        await database.ExecuteAsync(sql, new
        {
            Token = token,
            Username = username
        });

        return token;
    }

    public static async Task<string> ValidateToken(string token)
    {
        IAccess database = new Access();
        const string sql = "SELECT * FROM tokens WHERE Token = @Token LIMIT 1";
        List<TokenClass> results = await database.QueryAsync<TokenClass, dynamic>(sql, new
        {
            Token = token
        });
        return results.Count is 0 ? string.Empty : results.First().Username;
    }
    
    public static string HashUsername(string username)
    {
        var sb = new StringBuilder();
        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF32.GetBytes(username));
        foreach (byte b in hash) sb.Append(b.ToString("x2"));
        return sb.ToString().ToLowerInvariant();
    }
}