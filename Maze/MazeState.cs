using System.Collections.Concurrent;

namespace MazeRunner.Maze;

public class MazeState
{
    public Maze Maze { get; }
    public ConcurrentDictionary<Guid, Bot> Bots { get; } = new();

    public MazeState(int width = 10, int height = 10)
    {
        Maze = new Maze(width, height);
    }

    public Bot CreateBot(int x = 0, int y = 0, int energy = 100, string name = "", string team = "#ffffff")
    {
        var bot = new Bot(x, y, energy, name, team);
        Bots[bot.GetId()] = bot;
        return bot;
    }

    public Bot? GetBot(Guid id)
    {
        Bots.TryGetValue(id, out var bot);
        return bot;
    }
}
