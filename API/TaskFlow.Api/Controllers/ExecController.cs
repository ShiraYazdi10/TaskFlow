using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace TaskFlow.Api.Controllers;

public class ExecRequest
{
    public string ProcedureName { get; set; } = "";
    public Dictionary<string, JsonElement>? Parameters { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class ExecController : ControllerBase
{
    private readonly IConfiguration _config;
    public ExecController(IConfiguration config) => _config = config;

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ExecRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.ProcedureName))
            return BadRequest("procedureName is required");

        var cs = _config.GetConnectionString("Default");
        await using var conn = new SqlConnection(cs);

        var dp = new DynamicParameters();
        if (req.Parameters != null)
        {
            foreach (var kv in req.Parameters)
                dp.Add("@" + kv.Key, ConvertJsonElement(kv.Value));
        }

        var rows = await conn.QueryAsync(req.ProcedureName, dp, commandType: CommandType.StoredProcedure);
        return Ok(rows);
    }

    private static object? ConvertJsonElement(JsonElement el) => el.ValueKind switch
    {
        JsonValueKind.String => el.GetString(),
        JsonValueKind.Number => el.TryGetInt64(out var l) ? l : el.GetDecimal(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        _ => el.ToString()
    };
}
