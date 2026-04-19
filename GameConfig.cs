using System;
using Microsoft.Xna.Framework;

namespace SnakeGame;

public static class GameConfig
{
    public const int CellSize = 24;
    public const int GridWidth = 24;
    public const int GridHeight = 18;

    public static readonly TimeSpan MoveInterval = TimeSpan.FromMilliseconds(110);

    public static readonly Color BackgroundColor = new(18, 22, 30);
    public static readonly Color SnakeHeadColor = new(86, 214, 131);
    public static readonly Color SnakeBodyColor = new(48, 150, 96);
    public static readonly Color FoodColor = new(240, 96, 96);
}
