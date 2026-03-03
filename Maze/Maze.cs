namespace MazeRunner.Maze;

public class Maze
{
    private bool[][] _maze;

    public Maze(int width, int height)
    {
        _maze = GetNewMaze(width, height);
        MazeGrid = _maze;
    }

    public bool[][] MazeGrid {get; private set;}

    public (bool north, bool east, bool south, bool west) GetWalls(int x, int y)
    {
        return GetWalls(_maze, x, y);
    }

    public override string ToString()
    {
        return string.Join(",", _maze.Select(row => string.Join("", row.Select(cell => cell ? "1" : "0"))));
    }


    public static bool[][] GetNewMaze(int width, int height)
    {
        var maze = new bool[height*2+1][];
        var rand = new Random();

        for (int i = 0; i < maze.Length; i++)
        {
            maze[i] = new bool[width + 1];

            for (int j = 0; j < maze[i].Length; j++)
            {
                maze[i][j] = rand.NextDouble() > 0.5; // Initialize all walls as present
            }
        }

        return maze;
    }

    // The maze grid is twice as high as it is long to allow for the storage of the north and west walls in the same array.
    // The east and south walls are implied by the north and west walls of the adjacent cells.
    // Array indexes with y%2 == 0 store the north walls, and indexes with y%2 == 1 store the west walls.
    // The grid is also one cell larger in both dimensions to allow for the storage of the south and east walls of the outermost cells.
    public static (bool north, bool east, bool south, bool west) GetWalls(bool[][] maze, int x, int y)
    {
        if ((y + 1)*2 >= maze.Length || x + 1 >= maze[0].Length || y < 0 || x < 0)
        {
            throw new ArgumentOutOfRangeException("Coordinates are out of maze bounds.");
        }
            
        var north = maze[y*2][x];
        var east = maze[y*2 + 1][x + 1];
        var south = maze[(y + 1)*2][x];
        var west = maze[y*2 + 1][x];
        return (north, east, south, west);
    }
}