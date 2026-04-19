# SnakeGame

Classic Snake on a grid, built with **C#** and **MonoGame** (DesktopGL). Eat food to grow and score; hit a wall or yourself and it is game over.

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download)

## Run

From the repository root:

```bash
dotnet run --project SnakeGame.csproj
```

Or open `SnakeGame.sln` in Visual Studio / Rider and start debugging.

## Controls

| Input | Action |
|--------|--------|
| Arrow keys / **WASD** | Move |
| **Enter** | Restart (after game over) |
| **Esc** | Quit |

## Configuration

Grid size, move speed, and colors are in [`GameConfig.cs`](GameConfig.cs).
