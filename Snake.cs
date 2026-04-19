using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SnakeGame;

public enum MoveDirection
{
    Up,
    Down,
    Left,
    Right
}

public sealed class Snake
{
    private readonly List<Point> _segments = new();

    public MoveDirection CurrentDirection { get; private set; } = MoveDirection.Right;

    public MoveDirection? QueuedDirection { get; private set; }

    public IReadOnlyList<Point> Segments => _segments;

    public Snake(Point head, int length)
    {
        for (int i = 0; i < length; i++)
            _segments.Add(new Point(head.X - i, head.Y));
    }

    public void QueueDirection(MoveDirection direction)
    {
        QueuedDirection = direction;
    }

    public void ApplyQueuedDirection()
    {
        if (!QueuedDirection.HasValue)
            return;

        var next = QueuedDirection.Value;
        QueuedDirection = null;

        if (IsOpposite(next, CurrentDirection))
            return;

        CurrentDirection = next;
    }

    private static bool IsOpposite(MoveDirection a, MoveDirection b) =>
        (a == MoveDirection.Up && b == MoveDirection.Down) ||
        (a == MoveDirection.Down && b == MoveDirection.Up) ||
        (a == MoveDirection.Left && b == MoveDirection.Right) ||
        (a == MoveDirection.Right && b == MoveDirection.Left);

    public Point GetNextHead()
    {
        var head = _segments[0];
        return CurrentDirection switch
        {
            MoveDirection.Up => new Point(head.X, head.Y - 1),
            MoveDirection.Down => new Point(head.X, head.Y + 1),
            MoveDirection.Left => new Point(head.X - 1, head.Y),
            MoveDirection.Right => new Point(head.X + 1, head.Y),
            _ => head
        };
    }

    public void Step(Point newHead, bool grow)
    {
        _segments.Insert(0, newHead);
        if (!grow)
            _segments.RemoveAt(_segments.Count - 1);
    }
}
