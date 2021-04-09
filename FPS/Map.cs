using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace FPS
{
    public class Map
    {
        private Vector2 playerPosition = new Vector2(16, 16);
        private float playerAngle = 0;
        private float playerSpeed;
        private float playerRotation;

        private const float WalkSpeed = 8;
        private const float RotationSpeed = 1.5f;

        public void Update(GameTime gameTime, Game1 game)
        {
            ProcessKeyboardState(game);

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MovePlayer(delta);
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
