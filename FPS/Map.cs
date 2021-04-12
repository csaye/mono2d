using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace FPS
{
    public class Map
    {
        private const float Pi = (float)Math.PI;

        private bool[,] Walls;
        private const int Width = 32;
        private const int Height = 16;

        private Vector2 playerPosition = new Vector2(1, 1);
        private float playerAngle;
        private Vector2 playerDirection;
        private float playerRotation;

        private const float WalkSpeed = 8;
        private const float RotationSpeed = 1.5f;

        private const float Fov = Pi / 2;

        private const float RayStepDist = 0.1f;
        private const float MaxDepth = 64;

        private float fps;

        public Map()
        {
            string walls = "";
            walls += "################################";
            walls += "#..#.....#.....................#";
            walls += "#..#.....#.....................#";
            walls += "#..#..#..#.....#..########..####";
            walls += "#..#..#..#.....#.........#.....#";
            walls += "#..#..#######..#.........#.....#";
            walls += "#..............#..#......#.....#";
            walls += "#..............#..#......#.....#";
            walls += "#..#############..#......#.....#";
            walls += "#.................#......#.....#";
            walls += "#.................###..........#";
            walls += "#########..########............#";
            walls += "#..#.......#...........#########";
            walls += "#..............................#";
            walls += "#.....#.......#................#";
            walls += "#############################..#";

            // Initialize walls
            Walls = new bool[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Walls[x, y] = walls[y * Width + x] == '#';
                }
            }
        }

        public void Update(GameTime gameTime, Game1 game)
        {
            ProcessKeyboardState(game);

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MovePlayer(delta);
        }

        public void Draw(GameTime gameTime, Game1 game)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds; // Get time delta
            fps = 1 / delta; // Set fps

            // Draw background
            for (int y = 0; y < Drawing.GridHeight; y++)
            {
                Rectangle rect = new Rectangle(0, y * Drawing.Grid, Drawing.Width, Drawing.Grid);
                int colorFactor = (int)(128 * (Math.Abs(Drawing.GridHeight / 2 - y) / (float)(Drawing.GridHeight / 2)));
                Color color = new Color(colorFactor, colorFactor, colorFactor);
                Drawing.DrawRect(rect, color, game);
            }

            // Cast ray for each grid
            for (int x = 0; x < Drawing.GridWidth; x++)
            {
                // Calculate ray angle and starting position
                float rayAngle = playerAngle - (Fov / 2) + (Fov * ((float)x / Drawing.GridWidth));
                Vector2 rayPosition = playerPosition;
                float rayDistance = 0;
                bool hitWall = false;

                // Calculate ray step
                float rayStepX = RayStepDist * (float)Math.Sin(rayAngle);
                float rayStepY = RayStepDist * (float)Math.Cos(rayAngle);
                Vector2 rayStep = new Vector2(rayStepX, rayStepY);

                // Cast ray
                while (!hitWall && rayDistance < MaxDepth)
                {
                    // If in bounds, check whether wall hit
                    if (rayPosition.X >= 0 && rayPosition.X < Width && rayPosition.Y >= 0 && rayPosition.Y < Height)
                    {
                        int mapX = (int)rayPosition.X;
                        int mapY = (int)rayPosition.Y;
                        if (Walls[mapX, mapY]) hitWall = true;
                    }

                    // If wall not hit, increment ray distance
                    if (!hitWall)
                    {
                        rayPosition += rayStep;
                        rayDistance += RayStepDist;
                    }
                }

                // Get wall height and y
                float closeFactor = 1 - (rayDistance / MaxDepth);
                int wallHeight = (int)(Drawing.Height * closeFactor);
                int wallY = (Drawing.Height / 2) - (wallHeight / 2);

                // Draw wall rect
                Rectangle rect = new Rectangle(x * Drawing.Grid, wallY, Drawing.Grid, wallHeight);
                int colorFactor = (int)(255 * closeFactor);
                Color color = new Color(colorFactor, colorFactor, colorFactor);
                Drawing.DrawRect(rect, color, game);
            }

            // Draw data text
            Drawing.DrawText($"pos: {playerPosition}", new Vector2(8, 8), Color.White, game);
            Drawing.DrawText($"angle: {playerAngle}", new Vector2(8, 24), Color.White, game);
            Drawing.DrawText($"fps: {fps}", new Vector2(8, 40), Color.White, game);

            // Draw minimap
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    // If no wall, skip draw
                    if (!Walls[x, y]) continue;

                    // Draw wall
                    int posX = Drawing.Width - (Width + 8) + x;
                    int posY = 8 + y;
                    Rectangle rect = new Rectangle(posX, posY, 1, 1);
                    Drawing.DrawRect(rect, Color.White, game);
                }
            }
            // Draw player
            int playerX = Drawing.Width - (Width + 8) + (int)playerPosition.X;
            int playerY = 8 + (int)playerPosition.Y;
            Rectangle playerRect = new Rectangle(playerX - 1, playerY - 1, 3, 3);
            Drawing.DrawRect(playerRect, Color.Yellow, game);

            // Draw crosshair
            Rectangle crosshairRect = new Rectangle(Drawing.Width / 2 - 1, Drawing.Height / 2 - 4, 2, 8);
            Drawing.DrawRect(crosshairRect, Color.White, game);
            crosshairRect = new Rectangle(Drawing.Width / 2 - 4, Drawing.Height / 2 - 1, 8, 2);
            Drawing.DrawRect(crosshairRect, Color.White, game);
        }

        private void ProcessKeyboardState(Game1 game)
        {
            // Get keyboard state
            KeyboardState state = game.KeyboardState;

            // Get player direction
            if (state.IsKeyDown(Keys.W)) playerDirection.Y = 1;
            else if (state.IsKeyDown(Keys.S)) playerDirection.Y = -1;
            else playerDirection.Y = 0;
            if (state.IsKeyDown(Keys.D)) playerDirection.X = 1;
            else if (state.IsKeyDown(Keys.A)) playerDirection.X = -1;
            else playerDirection.X = 0;

            // Get rotation
            if (state.IsKeyDown(Keys.Left)) playerRotation = -1;
            else if (state.IsKeyDown(Keys.Right)) playerRotation = 1;
            else playerRotation = 0;
        }

        private void MovePlayer(float delta)
        {
            // Update angle
            playerAngle += playerRotation * RotationSpeed * delta;
            float sinA = (float)Math.Sin(playerAngle);
            float cosA = (float)Math.Cos(playerAngle);

            // Get x and y change
            float xChange = playerDirection.X * cosA + playerDirection.Y * sinA;
            float yChange = playerDirection.X * -sinA + playerDirection.Y * cosA;
            xChange *= WalkSpeed * delta;
            yChange *= WalkSpeed * delta;

            // If out of bounds or not wall, move player
            int mapX = (int)(playerPosition.X + xChange);
            int mapY = (int)(playerPosition.Y + yChange);
            if (mapX < 0 || mapX >= Width || mapY < 0 || mapY >= Height) playerPosition += new Vector2(xChange, yChange);
            else if (!Walls[mapX, mapY]) playerPosition += new Vector2(xChange, yChange);
        }
    }
}
