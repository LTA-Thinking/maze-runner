namespace MazeRunner.Maze;

public class Bot
{
    private int _x;
    private int _y;
    private int _energy;
    private Guid _id;

    public Bot(int x, int y, int energy)
    {
        _x = x;
        _y = y;
        _id = Guid.NewGuid();
        _energy = energy;
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

        var (north, east, south, west) = maze.GetWalls(_x, _y);
        bool canMove = direction switch
        {
            Direction.North => !north,
            Direction.East => !east,
            Direction.South => !south,
            Direction.West => !west,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), "Invalid direction")
        };

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


}