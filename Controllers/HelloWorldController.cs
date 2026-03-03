using Microsoft.AspNetCore.Mvc;

namespace MazeRunner.McpServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloWorldController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("hello world");
    }
}
