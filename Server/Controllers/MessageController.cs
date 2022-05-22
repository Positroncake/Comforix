using Comforix.Shared;
using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;

namespace Comforix.Server.Controllers;

[ApiController]
[Route("MessageApi")]
public class MessageController : ControllerBase
{
    [HttpPost]
    [Route("Send")]
    public async Task<ActionResult> SendMessage([FromBody] MessagePacket message)
    {
        // Validate token
        string senderUsername = await Utils.ValidateToken(message.Token);
        if (senderUsername.Equals(string.Empty)) return Unauthorized("Invalid token.");

        // Get table name (SHA-256 hash of username)
        string sender = Utils.HashUsername(senderUsername);

        // Store in sender database
        IAccess database = new Access();
        var sql = $"INSERT INTO `{sender}` (Username, Content, Sending, SentTime) VALUES (@Username, @Content, @Sending, @SentTime)";
        await database.ExecuteAsync(sql, new
        {
            Username = message.Dest,
            message.Content,
            Sending = true,
            SentTime = DateTime.UtcNow
        });

        // Store in recipient database
        var sql2 = $"INSERT INTO `{message.Dest}` (Username, Content, Sending, SentTime) VALUES (@Username, @Content, @Sending, @SentTime)";
        await database.ExecuteAsync(sql2, new
        {
            Username = sender,
            message.Content,
            Sending = false,
            SentTime = DateTime.UtcNow
        });

        return Ok();
    }

    [HttpGet]
    [Route("Get/{token}/{otherPerson}")]
    public async Task<ActionResult> GetMessages([FromRoute] string token, [FromRoute] string otherPerson)
    {
        string username = await Utils.ValidateToken(token);
        if (username.Equals(string.Empty)) return Unauthorized("Invalid token.");

        IAccess database = new Access();
        string hash = Utils.HashUsername(username);
        var sql = $"SELECT * FROM `{hash}` WHERE Username = @Username";
        List<MessageDb> messages = await database.QueryAsync<MessageDb, dynamic>(sql, new
        {
            Username = otherPerson
        });
        
        return Ok(messages);
    }
}