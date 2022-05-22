using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Comforix.Shared;
using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;

namespace Comforix.Server.Controllers;

[ApiController]
[Route("AccountApi")]
public class AccountController : ControllerBase
{
    [HttpPost]
    [Route("New")]
    public async Task<ActionResult> NewAcc([FromBody] Account account)
    {
        IAccess database = new Access();

        // Check if username is taken
        const string check = "SELECT * FROM accounts WHERE Username = @Username LIMIT 1";
        List<Account> result = await database.QueryAsync<Account, dynamic>(check, new
        {
            account.Username
        });
        if (result.Count is 1) return Conflict("Username already exists");

        // Store new account into database
        const string command =
            "INSERT INTO accounts (Username, PasswordHash, PasswordSalt, Level1, Level2, Level3, Reputation) VALUES (@Username, @PasswordHash, @PasswordSalt, @Level1, @Level2, @Level3, @Reputation)";
        await database.ExecuteAsync(command, new
        {
            account.Username,
            account.PasswordHash,
            account.PasswordSalt,
            account.Level1,
            account.Level2,
            account.Level3,
            Reputation = 9
        });
        
        // Create database table for user
        string sha256Hash = Utils.HashUsername(account.Username);
        var sql = @$"CREATE TABLE `{sha256Hash}` (
    Username TINYTEXT,
    Content TEXT,
    Sending BOOL,
    SentTime DATETIME
)";
        await database.ExecuteAsync(sql, new
        {
            TableName = sha256Hash
        });

        // Create token
        string token = await Utils.NewToken(account.Username);
        return Ok(token);
    }

    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult> LoginToAcc([FromBody] LoginReq request)
    {
        // Check if account exists. If exists, retrieve
        IAccess database = new Access();
        const string sql = "SELECT * FROM accounts WHERE Username = @Username LIMIT 1";
        List<Account> accounts = await database.QueryAsync<Account, dynamic>(sql, new
        {
            Username = request.Username
        });
        if (accounts.Count is not 1) return Unauthorized();
        Account selected = accounts.First();

        // Salt
        byte[] hash = request.Password.Concat(selected.PasswordHash).ToArray();
        
        // Hash
        using var hasher = SHA512.Create();
        hash = hasher.ComputeHash(hash);
        
        // Compare
        if (hash.SequenceEqual(selected.PasswordHash) is false) return Unauthorized();
        string token = await Utils.NewToken(selected.Username);
        return Ok(token);
    }

    [HttpGet]
    [Route("FindUsers/{level1Filter:int}/{level2Filter:int}")]
    public async Task<ActionResult> FindUsersWithFilter([FromRoute] int level1Filter, [FromRoute] int level2Filter)
    {
        IAccess database = new Access();
        const string sql = "SELECT * FROM accounts WHERE Level1 = @Level1";
        List<Account> matches = await database.QueryAsync<Account, dynamic>(sql, new
        {
            Level1 = level1Filter
        });

        List<Account> filtered = matches.Where(account => account.Level2.Contains("\u001f" + level2Filter + "\u001f")).ToList();
        return Ok(filtered);
    }
}