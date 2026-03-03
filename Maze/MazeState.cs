namespace MazeRunner.Maze;

public class MazeState
{
    public Maze Maze { get; }

    public MazeState(int width = 10, int height = 10)
    {
        Maze = new Maze(width, height);
    }
}
