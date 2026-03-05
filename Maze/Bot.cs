namespace MazeRunner.Maze;

public class Bot
{
    private int _x;
    private int _y;
    private int _energy;
    private Guid _id;
    private string _name;

    public Bot(int x, int y, int energy, string name = "")
    {
        _x = x;
        _y = y;
        _id = Guid.NewGuid();
        _energy = energy;
        _name = string.IsNullOrWhiteSpace(name) ? $"Bot-{_id.ToString().Substring(0, 4)}" : name;
    }

    public string GetName()
    {
        return _name;
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public (int x, int y) GetPosition()
    {
        return (_x, _y);
    }

    public int GetEnergy()
    {
        return _energy;
    }

    public Guid GetId()
    {
        return _id;
    }

    public bool TryMove(Direction direction, Maze maze)
    {
        if (_energy <= 0)
        {
            return false; // Bot has no energy left to move
        }

        var walls = maze.GetWalls(_x, _y);
        bool canMove = !walls[(int)direction]; // Check if there's no wall in the desired direction

        if (canMove)
        {
            switch (direction)
            {
                case Direction.North:
                    _y -= 1;
                    break;
                case Direction.East:
                    _x += 1;
                    break;
                case Direction.South:
                    _y += 1;
                    break;
                case Direction.West:
                    _x -= 1;
                    break;
            }
            _energy -= 1; // Decrease energy by 1 for each move
            return true; // Move successful
        }

        return false; // Move blocked by a wall
    }

    public Maze Scan(Maze maze, int range)
    {
        var width = Math.Min(Math.Min(range * 2 + 1, maze.Width), Math.Min(_x + range + 1, maze.Width - _x + range));
        var height = Math.Min(Math.Min(range * 2 + 1, maze.Height), Math.Min(_y + range + 1, maze.Height - _y + range));

        var section = new bool[height*2+1][];
        var mazeXCenter = _x;
        var mazeYCenter = _y * 2;
        for (int i = 0; i < section.Length; i++)
        {
            section[i] = new bool[width+1];
            for (int j = 0; j < section[i].Length; j++)
            {
                int mazeX = mazeXCenter - range + j;
                int mazeY = mazeYCenter - range + i;
                if (mazeX >= 0 && mazeX < maze.MazeGrid[0].Length && mazeY >= 0 && mazeY < maze.MazeGrid.Length)
                {
                    section[i][j] = maze.MazeGrid[mazeY][mazeX];
                }
                else
                {
                    section[i][j] = true; // Treat out-of-bounds as walls
                }
            }
        }

        return new Maze(section);
    }

    public string ToJson()
    {
        return $"{{\"id\":\"{_id}\",\"name\":\"{_name}\",\"x\":{_x},\"y\":{_y},\"energy\":{_energy}}}";
    }

}