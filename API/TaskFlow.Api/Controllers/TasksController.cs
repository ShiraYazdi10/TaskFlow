using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IConfiguration _config;

    public TasksController(IConfiguration config)
    {
        _config = config;
    }

    private SqlConnection CreateConnection()
        => new SqlConnection(_config.GetConnectionString("Default"));

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        using var conn = CreateConnection();
        var result = await conn.QueryAsync(
            "dbo.Tasks_GetAll",
            new { Search = (string?)null },
            commandType: CommandType.StoredProcedure
        );
        return Ok(result);
    }

   [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        using var conn = CreateConnection();
        var result = await conn.QueryFirstOrDefaultAsync(
            "dbo.Tasks_GetById",
            new { Id = id },
            commandType: CommandType.StoredProcedure
        );
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] dynamic body)
    {
        using var conn = CreateConnection();
        var newId = await conn.QuerySingleAsync<int>(
            "dbo.Tasks_Create",
            new
            {
                Title = (string)body.title,
                Description = (string?)body.description,
                StatusId = (int)body.statusId,
                PriorityId = (int)body.priorityId
            },
            commandType: CommandType.StoredProcedure
        );
        return Ok(new { id = newId });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] dynamic body)
    {
        using var conn = CreateConnection();
        var result = await conn.QuerySingleAsync(
            "dbo.Tasks_Update",
            new
            {
                Id = id,
                Title = (string)body.title,
                Description = (string?)body.description,
                StatusId = (int)body.statusId,
                PriorityId = (int)body.priorityId
            },
            commandType: CommandType.StoredProcedure
        );
        return Ok(result);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] dynamic body)
    {
        using var conn = CreateConnection();
        var result = await conn.QuerySingleAsync(
            "dbo.Tasks_ChangeStatus",
            new
            {
                Id = id,
                StatusId = (int)body.statusId
            },
            commandType: CommandType.StoredProcedure
        );
        return Ok(result);
    }
  [HttpDelete("{id:int}")]
public async Task<IActionResult> Delete(int id)
{
    using var conn = CreateConnection();
    var result = await conn.QuerySingleAsync(
        "dbo.Tasks_Delete",
        new { Id = id },
        commandType: CommandType.StoredProcedure
    );
    return Ok(result);
}

}
