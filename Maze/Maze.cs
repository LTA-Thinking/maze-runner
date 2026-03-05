using System.Runtime;

namespace MazeRunner.Maze;

public class Maze
{
    private bool[][] _maze;

    public Maze(int width, int height)
    {
        _maze = GetNewMaze(width, height);
        MazeGrid = _maze;
    }

    public Maze(bool[][] maze)
    {
        _maze = maze;
        MazeGrid = maze;
    }

    public bool[][] MazeGrid {get; private set;}

    public int Width => _maze[0].Length - 1; // The maze grid is one cell larger than the maze dimensions to allow for the storage of the south and east walls of the outermost cells.
    public int Height => (_maze.Length - 1) / 2; // The maze grid is twice as high as it is long to allow for the storage of the north and west walls in the same array.

    public bool[] GetWalls(int x, int y)
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
                maze[i][j] = true; // Initialize all walls as present
            }
        }

        var grid = new bool[height][];
        for (int i = 0; i < height; i++)
        {
            grid[i] = new bool[width];
            for (int j = 0; j < width; j++)
            {
                grid[i][j] = false;
            }
        }

        var targetX = rand.Next(width);
        var targetY = rand.Next(height);
        grid[targetY][targetX] = true; // Ensure there's a path to the target
        var set = new HashSet<(int x, int y)>();
        set.Add((targetX, targetY));

        while (set.Count > 0)
        {
            var (x, y) = set.ElementAt(rand.Next(set.Count));
            var direction = (Direction)rand.Next(4);
            var removedWall = false;

            for (int i = 0; i < 4; i++)
            {
                if (!IsVisited(grid, x, y, direction))
                {
                    RemoveWall(maze, x, y, direction);
                    var (newX, newY) = direction switch
                    {
                        Direction.North => (x, y - 1), // North
                        Direction.East => (x + 1, y), // East
                        Direction.South => (x, y + 1), // South
                        Direction.West => (x - 1, y), // West
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    grid[newY][newX] = true; // Mark the cell as visited
                    set.Add((newX, newY));
                    removedWall = true;
                    break;
                }
                direction = (Direction)(((int)direction + 1) % 4); // Rotate to the next direction
            }

            if (!removedWall)
            {
                set.Remove((x, y)); // All directions visited, remove from set
            }
        }

        return maze;
    }

    private static bool IsVisited(bool[][] maze, int x, int y, Direction direction)
    {
        return direction switch
        {
            Direction.North => y - 1 < 0 || maze[y - 1][x],
            Direction.East => x + 1 >= maze[0].Length || maze[y][x + 1],
            Direction.South => y + 1 >= maze.Length || maze[y + 1][x],
            Direction.West => x - 1 < 0 || maze[y][x - 1],
            _ => throw new ArgumentOutOfRangeException(nameof(direction), "Invalid direction")
        };
    }

    private static void RemoveWall(bool[][] maze, int x, int y, Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                maze[y*2][x] = false; // Remove north wall
                break;
            case Direction.East:
                maze[y*2 + 1][x + 1] = false; // Remove east wall
                break;
            case Direction.South:
                maze[(y + 1)*2][x] = false; // Remove south wall
                break;
            case Direction.West:
                maze[y*2 + 1][x] = false; // Remove west wall
                break;
        }
    }

    // The maze grid is twice as high as it is long to allow for the storage of the north and west walls in the same array.
    // The east and south walls are implied by the north and west walls of the adjacent cells.
    // Array indexes with y%2 == 0 store the north walls, and indexes with y%2 == 1 store the west walls.
    // The grid is also one cell larger in both dimensions to allow for the storage of the south and east walls of the outermost cells.
    public static bool[] GetWalls(bool[][] maze, int x, int y)
    {
        if ((y + 1)*2 >= maze.Length || x + 1 >= maze[0].Length || y < 0 || x < 0)
        {
            throw new ArgumentOutOfRangeException("Coordinates are out of maze bounds.");
        }
            
        var north = maze[y*2][x];
        var east = maze[y*2 + 1][x + 1];
        var south = maze[(y + 1)*2][x];
        var west = maze[y*2 + 1][x];
        return new bool[] { north, east, south, west };
    }
}