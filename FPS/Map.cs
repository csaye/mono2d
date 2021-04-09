using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace FPS
{
    public class Map
    {
        private bool[,] map;

        private Vector2 playerPosition = new Vector2(16, 16);
        private float playerAngle = 0;
        private float playerSpeed;
        private float playerRotation;

        private const float WalkSpeed = 8;
        private const float RotationSpeed = 1.5f;

        private const float Fov = (float)Math.PI / 4;

        private const float RayStep = 0.1f;
        private const float MaxDepth = Drawing.GridWidth;

        public Map()
        {
            int width = Drawing.GridWidth;
            int height = Drawing.GridHeight;
            map = new bool[width, height];
        }

        public void Update(GameTime gameTime, Game1 game)
        {
            ProcessKeyboardState(game);

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MovePlayer(delta);
        }

        public void Draw(Game1 game)
        {
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

                // Cast ray
                while (!hitWall && rayDistance < MaxDepth)
                {
                    // If out of bounds, wall hit
                    if (rayPosition.X < 0 || rayPosition.X >= Drawing.GridWidth || rayPosition.Y < 0 || rayPosition.Y >= Drawing.GridHeight)
                    {
                        hitWall = true;
                    }
                    // If in bounds, check whether wall hit
                    else
                    {
                        int mapX = (int)rayPosition.X;
                        int mapY = (int)rayPosition.Y;
                        if (map[mapX, mapY]) hitWall = true;
                    }

                    // If wall not hit, increment ray distance
                    if (!hitWall)
                    {
                        rayPosition.X += RayStep * (float)Math.Sin(rayAngle);
                        rayPosition.Y += RayStep * (float)Math.Cos(rayAngle);
                        rayDistance += RayStep;
                    }
                }

                // Get wall height and y
                float closeFactor = (1 - (rayDistance / MaxDepth));
                int wallHeight = (int)(Drawing.Height * closeFactor);
                int wallY = (Drawing.Height / 2) - (wallHeight / 2);

                // Draw wall rect
                Rectangle rect = new Rectangle(x * Drawing.Grid, wallY, Drawing.Grid, wallHeight);
                int colorFactor = (int)(255 * closeFactor);
                Color color = new Color(colorFactor, colorFactor, colorFactor);
                Drawing.DrawRect(rect, color, game);
            }
        }

        private void ProcessKeyboardState(Game1 game)
        {
            // Get keyboard state
            KeyboardState state = game.KeyboardState;

            // Get speed
            if (state.IsKeyDown(Keys.W)) playerSpeed = 1;
            else if (state.IsKeyDown(Keys.S)) playerSpeed = -1;
            else playerSpeed = 0;
            // Get rotation
            if (state.IsKeyDown(Keys.A)) playerRotation = -1;
            else if (state.IsKeyDown(Keys.D)) playerRotation = 1;
            else playerRotation = 0;
        }

        private void MovePlayer(float delta)
        {
            // Update angle
            playerAngle += playerRotation * RotationSpeed * delta;

            // Get x and y change
            float xChange = playerSpeed * WalkSpeed * delta * (float)Math.Sin(playerAngle);
            float yChange = playerSpeed * WalkSpeed * delta * (float)Math.Cos(playerAngle);
            int mapX = (int)(playerPosition.X + xChange);
            int mapY = (int)(playerPosition.Y + yChange);

            // If out of bounds, return
            if (mapX < 0 || mapX > map.GetLength(0) - 1 || mapY < 0 || mapY > map.GetLength(1) - 1) return;
            // If not wall, move player
            if (!map[mapX, mapY]) playerPosition += new Vector2(xChange, yChange);
        }
    }
}
