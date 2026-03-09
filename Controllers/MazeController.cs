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

    [HttpPost("team/{team}/bot")]
    public IActionResult CreateBot(string team, [FromQuery] int x = 0, [FromQuery] int y = 0, [FromQuery] int energy = 100, [FromQuery] string name = "")
    {
        var hexTeam = "#" + team;
        var bot = _mazeState.CreateBot(x, y, energy, name, hexTeam);
        return Created($"api/maze/team/{team}/bot/{bot.GetId()}", new
        {
            id = bot.GetId(),
            name = bot.GetName(),
            team = bot.GetTeam(),
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
            team = b.GetTeam(),
            x = b.GetPosition().x,
            y = b.GetPosition().y,
            energy = b.GetEnergy()
        });
        return Ok(bots);
    }

    [HttpGet("team/{team}/bots")]
    public IActionResult GetBotsByTeam(string team)
    {
        var hexTeam = "#" + team;
        var bots = _mazeState.Bots.Values
            .Where(b => b.GetTeam().Equals(hexTeam, StringComparison.OrdinalIgnoreCase))
            .Select(b => new
            {
                id = b.GetId(),
                name = b.GetName(),
                team = b.GetTeam(),
                x = b.GetPosition().x,
                y = b.GetPosition().y,
                energy = b.GetEnergy()
            });
        return Ok(bots);
    }

    [HttpGet("bot/{id}")]
    public IActionResult GetBot(Guid id)
    {
        var bot = _mazeState.GetBot(id);
        if (bot == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            id = bot.GetId(),
            name = bot.GetName(),
            team = bot.GetTeam(),
            x = bot.GetPosition().x,
            y = bot.GetPosition().y,
            energy = bot.GetEnergy()
        });
    }

    [HttpPost("bot/{id}/command")]
    public IActionResult CommandBot(Guid id, [FromQuery] string command)
    {
        var bot = _mazeState.GetBot(id);
        if (bot == null)
        {
            return NotFound();
        }

        var parameters = command.Split(' ');

        switch (parameters[0].ToLower())
        {
            case "move":
                if (parameters.Length < 2 || !Enum.TryParse<Direction>(parameters[1], true, out var direction))
                {
                    return BadRequest("Invalid move command. Usage: move <direction>");
                }

                if (!bot.TryMove(direction, _mazeState.Maze))
                {
                    return BadRequest("Bot cannot move in that direction.");
                }

                return Ok();
            case "scan":
                if (parameters.Length < 2 || !int.TryParse(parameters[1], out var radius))
                {
                    return BadRequest("Invalid scan command. Usage: scan <radius>");
                }

                var result = bot.Scan(_mazeState.Maze, radius);
                return Ok(result.ToString());
            default:
                return BadRequest("Unknown command.");
        }
    }
}
