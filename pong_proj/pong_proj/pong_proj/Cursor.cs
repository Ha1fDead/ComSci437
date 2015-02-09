using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pong_proj
{
    class Cursor : IEntity2D
    {
        protected Texture2D EntityTexture;
        protected Vector2 Position;
        protected int width;
        protected int height;
        protected Vector2 Velocity;

        public Cursor()
        {
            Console.Write("Test of github sync");
        }

        public void Initialize(Texture2D texture, Vector2 position, int width, int height, Vector2 velocity)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.width = width;
            this.height = height;
            this.EntityTexture = texture;
        }

        public void Update(GameTime gameTime)
        {
            Position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(EntityTexture, Position, Color.White);
        }

    }
}
