using Microsoft.Xna.Framework;
using System;

namespace Mono2D
{
    public class Map
    {
        private const float Pi = (float)Math.PI;

        private bool[,] Walls;
        private const int Width = 64;
        private const int Height = 64;

        private const float Fov = Pi / 2;

        private const float RayStepDist = 0.1f;
        private const float MaxDepth = 64;

        private float fps;

        public bool WallAt(int x, int y)
        {
            // If out of bounds, return false
            if (x < 0 || x >= Width || y < 0 || y >= Height) return false;
            // If in bounds, return whether wall at position
            else return Walls[x, y];
        }

        public Map()
        {
            //string walls = "";
            //walls += "################################";
            //walls += "#..#.....#.....................#";
            //walls += "#..#.....#.....................#";
            //walls += "#..#..#..#.....#..########..####";
            //walls += "#..#..#..#.....#.........#.....#";
            //walls += "#..#..#######..#.........#.....#";
            //walls += "#..............#..#......#.....#";
            //walls += "#..............#..#......#.....#";
            //walls += "#..#############..#......#.....#";
            //walls += "#.................#......#.....#";
            //walls += "#.................###..........#";
            //walls += "#########..########............#";
            //walls += "#..#.......#...........#########";
            //walls += "#..............................#";
            //walls += "#.....#.......#................#";
            //walls += "#############################..#";

            // Initialize walls
            Walls = new bool[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Walls[x, y] = x % 8 == 0 && y % 8 == 0;
                    //Walls[x, y] = walls[y * Width + x] == '#';
                }
            }
        }

        public void Draw(GameTime gameTime, Game1 game)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds; // Get time delta
            fps = 1 / delta; // Set fps

            Vector2 position = game.Player.GetPosition();
            float angle = game.Player.GetAngle();

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
                float rayAngle = angle - (Fov / 2) + (Fov * ((float)x / Drawing.GridWidth));
                Vector2 rayPosition = position;
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
            Drawing.DrawText($"pos: {position}", new Vector2(8, 8), Color.White, game);
            Drawing.DrawText($"angle: {angle}", new Vector2(8, 24), Color.White, game);
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
            int playerX = Drawing.Width - (Width + 8) + (int)position.X;
            int playerY = 8 + (int)position.Y;
            Rectangle playerRect = new Rectangle(playerX - 1, playerY - 1, 3, 3);
            Drawing.DrawRect(playerRect, Color.Yellow, game);

            // Draw crosshair
            Rectangle crosshairRect = new Rectangle(Drawing.Width / 2 - 1, Drawing.Height / 2 - 4, 2, 8);
            Drawing.DrawRect(crosshairRect, Color.White, game);
            crosshairRect = new Rectangle(Drawing.Width / 2 - 4, Drawing.Height / 2 - 1, 8, 2);
            Drawing.DrawRect(crosshairRect, Color.White, game);
        }
    }
}
