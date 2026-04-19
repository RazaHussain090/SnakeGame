using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SnakeGame;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixel = null!;

    private readonly FoodSpawner _foodSpawner = new();
    private Snake _snake = null!;
    private Point? _food;
    private TimeSpan _moveAccumulator;

    private KeyboardState _previousKeyboard;

    private bool _gameOver;
    private int _score;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = GameConfig.GridWidth * GameConfig.CellSize;
        _graphics.PreferredBackBufferHeight = GameConfig.GridHeight * GameConfig.CellSize;
        _graphics.ApplyChanges();

        StartNewGame();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Escape))
            Exit();

        if (_gameOver)
        {
            if (WasPressed(keyboard, Keys.Enter))
                StartNewGame();

            _previousKeyboard = keyboard;
            base.Update(gameTime);
            return;
        }

        ReadDirectionInput(keyboard);

        _moveAccumulator += gameTime.ElapsedGameTime;
        while (_moveAccumulator >= GameConfig.MoveInterval)
        {
            _moveAccumulator -= GameConfig.MoveInterval;
            TickMove();
            if (_gameOver)
                break;
        }

        _previousKeyboard = keyboard;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(GameConfig.BackgroundColor);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        if (_food.HasValue)
            DrawCell(_food.Value, GameConfig.FoodColor);

        var segments = _snake.Segments;
        for (var i = 0; i < segments.Count; i++)
        {
            var color = i == 0 ? GameConfig.SnakeHeadColor : GameConfig.SnakeBodyColor;
            DrawCell(segments[i], color);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private bool WasPressed(KeyboardState current, Keys key) =>
        current.IsKeyDown(key) && _previousKeyboard.IsKeyUp(key);

    private void ReadDirectionInput(KeyboardState keyboard)
    {
        if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
            _snake.QueueDirection(MoveDirection.Up);
        else if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
            _snake.QueueDirection(MoveDirection.Down);
        else if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
            _snake.QueueDirection(MoveDirection.Left);
        else if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
            _snake.QueueDirection(MoveDirection.Right);
    }

    private void TickMove()
    {
        _snake.ApplyQueuedDirection();

        var newHead = _snake.GetNextHead();
        var willEat = _food.HasValue && newHead == _food.Value;

        if (IsWallCollision(newHead))
        {
            EndGame();
            return;
        }

        if (CollidesWithBody(newHead, willEat))
        {
            EndGame();
            return;
        }

        _snake.Step(newHead, willEat);

        if (willEat)
        {
            _score++;
            SpawnFood();
        }

        UpdateWindowTitle();
    }

    private static bool IsWallCollision(Point head) =>
        head.X < 0 || head.Y < 0 ||
        head.X >= GameConfig.GridWidth || head.Y >= GameConfig.GridHeight;

    private bool CollidesWithBody(Point newHead, bool willGrow)
    {
        var segments = _snake.Segments;
        var lastIndex = segments.Count - 1;

        for (var i = 1; i < segments.Count; i++)
        {
            if (!willGrow && i == lastIndex)
                continue;

            if (segments[i] == newHead)
                return true;
        }

        return false;
    }

    private void EndGame()
    {
        _gameOver = true;
        Window.Title = $"Snake — Score: {_score} — Game Over — Enter: restart";
    }

    private void StartNewGame()
    {
        var head = new Point(GameConfig.GridWidth / 2, GameConfig.GridHeight / 2);
        _snake = new Snake(head, 3);
        _score = 0;
        _gameOver = false;
        _moveAccumulator = TimeSpan.Zero;
        SpawnFood();
        UpdateWindowTitle();
    }

    private void SpawnFood()
    {
        _food = _foodSpawner.TrySpawn(GameConfig.GridWidth, GameConfig.GridHeight, _snake.Segments);
    }

    private void UpdateWindowTitle()
    {
        if (_gameOver)
            return;

        Window.Title = $"Snake — Score: {_score} — Arrows/WASD — Esc: quit";
    }

    private void DrawCell(Point cell, Color color)
    {
        var size = GameConfig.CellSize;
        var rect = new Rectangle(cell.X * size, cell.Y * size, size - 1, size - 1);
        _spriteBatch.Draw(_pixel, rect, color);
    }
}
