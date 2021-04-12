# Mono2D
A 2D raycasting engine built in MonoGame.

![](https://user-images.githubusercontent.com/27871609/114435551-85095580-9b81-11eb-857f-f007c67778bd.gif)

## Controls

W/S: move forwards/backwards\
A/D: move left/right\
Left/Right: look left/right

## Installation/Running

Ensure [Visual Studio](https://visualstudio.microsoft.com/downloads/) and [MonoGame](https://www.monogame.net/downloads/) are installed.

Clone the project by running `git clone https://github.com/csaye/mono2d`

Open in Visual Studio and press the big play button or use F5 to run.

## Settings

[Drawing.cs](Mono2D/Drawing.cs)
```cs
public const int Grid = 1; // Size of grid
public const int GridWidth = 512; // Width of grid
public const int GridHeight = 512; // Height of grid
```

[Map.cs](Mono2D/Map.cs)
```cs
private const int Width = 64; // Width of map (x)
private const int Height = 64; // Height of map (y)

private const float Fov = Pi / 2; // Base field of view

private const float RayStepDist = 0.1f; // Distance between ray steps
private const float MaxDepth = 64; // Maximum ray depth
```

[Player.cs](Mono2D/Player.cs)
```cs
private const float Speed = 8; // Base walk speed
private const float Spin = 1.5f; // Base rotation speed
```
