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
}
