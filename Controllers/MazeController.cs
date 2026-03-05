using Microsoft.AspNetCore.Mvc;
using MazeRunner.Maze;

namespace MazeRunner.McpServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MazeController : ControllerBase
{
    private readonly MazeState _mazeState;

    public MazeController(MazeState mazeState)
    {
        _mazeState = mazeState;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_mazeState.Maze.ToString());
    }

    [HttpPost("bot")]
    public IActionResult CreateBot([FromQuery] int x = 0, [FromQuery] int y = 0, [FromQuery] int energy = 100, [FromQuery] string name = "")
    {
        var bot = _mazeState.CreateBot(x, y, energy, name);
        return Created($"api/maze/bot/{bot.GetId()}", new
        {
            id = bot.GetId(),
            name = bot.GetName(),
            x = bot.GetPosition().x,
            y = bot.GetPosition().y,
            energy = bot.GetEnergy()
        });
    }

    [HttpGet("bots")]
    public IActionResult GetBots()
    {
        var bots = _mazeState.Bots.Values.Select(b => new
        {
            id = b.GetId(),
            name = b.GetName(),
            x = b.GetPosition().x,
            y = b.GetPosition().y,
            energy = b.GetEnergy()
        });
        return Ok(bots);
    }
}
