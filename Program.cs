using MazeRunner.Maze;
using ModelContextProtocol;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MazeState>();
builder.Services.AddControllers();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Display"))
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Display"))
});

app.MapControllers();
app.MapMcp();

app.Run();
