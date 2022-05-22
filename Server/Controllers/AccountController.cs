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
        // Get account from database if exists
        IAccess access = new Access();
        const string sql = "SELECT * FROM accounts WHERE Username = @LoginUsername LIMIT 1";
        List<Account> accounts = await access.QueryAsync<Account, dynamic>(sql, new
        {
            LoginUsername = request.Username
        });
        
        // Check if account exists
        Account selected;
        if (accounts.Count == 1) selected = accounts.First();
        else return Unauthorized();

        // Add salt to inputted password
        byte[] hash = request.Password.Concat(selected.PasswordSalt).ToArray();
        
        // Hash inputted password
        var sha512 = SHA512.Create();
        for (var i = 0; i < 5000; ++i) hash = sha512.ComputeHash(hash);
        
        // Validation - compare inputted hash against hash in database
        if (hash.SequenceEqual(selected.PasswordHash) is false) return Unauthorized();
        string token = await Utils.NewToken(selected.Username);
        return Ok(token);
    }

    [HttpPost]
    [Route("Vote")]
    public async Task<ActionResult> ModifyUserReputation([FromBody] RepChangeRequest request)
    {
        if (request.Vote is < -3 or > 3) return BadRequest();
        
        IAccess database = new Access();
        const string getCurrentRep = "SELECT * FROM accounts WHERE Username = @Username LIMIT 1";
        List<Account> result = await database.QueryAsync<Account, dynamic>(getCurrentRep, new
        {
            Username = request.Username
        });
        int reputation = result.First().Reputation;

        const string setNewRep = "UPDATE accounts SET Reputation = @Reputation WHERE Username = @Username";
        await database.ExecuteAsync(setNewRep, new
        {
            Reputation = reputation + request.Vote,
            Username = request.Username
        });

        return Ok();
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
        foreach (Account x in filtered)
        {
            x.PasswordHash = Array.Empty<byte>();
            x.PasswordSalt = Array.Empty<byte>();
        }
        
        return Ok(filtered);
    }

    [HttpGet]
    [Route("GetAccountInfo/{token}")]
    public async Task<ActionResult> GetAccountInfoFromToken([FromRoute] string token)
    {
        string username = await Utils.ValidateToken(token);
        if (username.Equals(string.Empty)) return Unauthorized();

        IAccess database = new Access();
        const string sql = "SELECT * FROM accounts WHERE Username = @Username";
        List<Account> accounts = await database.QueryAsync<Account, dynamic>(sql, new
        {
            Username = username
        });

        Account account = accounts.First();
        return Ok(account);
    }
}