using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SnakeGame;

public sealed class FoodSpawner
{
    private readonly Random _random = new();

    public Point? TrySpawn(int gridWidth, int gridHeight, IReadOnlyList<Point> occupied)
    {
        var cells = gridWidth * gridHeight;
        if (occupied.Count >= cells)
            return null;

        for (var attempt = 0; attempt < cells * 4; attempt++)
        {
            var candidate = new Point(_random.Next(gridWidth), _random.Next(gridHeight));
            if (!Contains(occupied, candidate))
                return candidate;
        }

        for (var y = 0; y < gridHeight; y++)
        {
            for (var x = 0; x < gridWidth; x++)
            {
                var p = new Point(x, y);
                if (!Contains(occupied, p))
                    return p;
            }
        }

        return null;
    }

    private static bool Contains(IReadOnlyList<Point> occupied, Point point)
    {
        for (var i = 0; i < occupied.Count; i++)
        {
            if (occupied[i] == point)
                return true;
        }

        return false;
    }
}
