using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace FPS
{
    public class Player
    {
        private Vector2 position = new Vector2(1, 1);
        private float angle;
        private Vector2 direction;
        private float rotation;

        private const float Speed = 8; // Base walk speed
        private const float Spin = 1.5f; // Base rotation speed

        public Vector2 GetPosition() => position;
        public float GetAngle() => angle;

        public Player() { }

        public void Update(GameTime gameTime, Game1 game)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds; // Get time delta
            ProcessKeyboardState(game); // Process keyboard state
            MovePlayer(delta, game); // Move player by delta
        }

        private void ProcessKeyboardState(Game1 game)
        {
            // Get keyboard state
            KeyboardState state = game.KeyboardState;

            // Get player direction
            if (state.IsKeyDown(Keys.W)) direction.Y = 1;
            else if (state.IsKeyDown(Keys.S)) direction.Y = -1;
            else direction.Y = 0;
            if (state.IsKeyDown(Keys.D)) direction.X = 1;
            else if (state.IsKeyDown(Keys.A)) direction.X = -1;
            else direction.X = 0;

            // Get rotation
            if (state.IsKeyDown(Keys.Left)) rotation = -1;
            else if (state.IsKeyDown(Keys.Right)) rotation = 1;
            else rotation = 0;
        }

        private void MovePlayer(float delta, Game1 game)
        {
            // Update angle
            angle += rotation * Spin * delta;
            float sinA = (float)Math.Sin(angle);
            float cosA = (float)Math.Cos(angle);

            // Get x and y change
            float xChange = direction.X * cosA + direction.Y * sinA;
            float yChange = direction.X * -sinA + direction.Y * cosA;
            xChange *= Speed * delta;
            yChange *= Speed * delta;

            // If no wall at position, move player
            int mapX = (int)(position.X + xChange);
            int mapY = (int)(position.Y + yChange);
            if (!game.Map.WallAt(mapX, mapY)) position += new Vector2(xChange, yChange);
        }
    }
}
