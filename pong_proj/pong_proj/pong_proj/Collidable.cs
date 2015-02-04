using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pong_proj
{
    /// <summary>
    /// 
    /// </summary>
    class Component2D : ICollidable
    {
        /// <summary>
        /// Up to 4, from PlayerIndex
        /// </summary>
        protected Texture2D EntityTexture;
        protected Vector2 Position;
        protected int width;
        protected int height;
        protected Vector2 Velocity;
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int) Position.X, (int) Position.Y, width, height);
            }
        }


        public Component2D()
        {

        }

        public void Initialize(Texture2D texture, Vector2 position, int width, int height, Vector2 velocity)
        {
            this.Position = position;
            this.width = width;
            this.height = height;
            this.Velocity = velocity;
            this.EntityTexture = texture;
        }

        public void Update(GameTime gameTime)
        {
            this.Position.X += this.Velocity.X;
            this.Position.Y += this.Velocity.Y;
        }

        public Rectangle CalculateBoundingBox()
        {
            return new Rectangle((int) Position.X, (int) Position.Y, 10, 10);
        }

        public bool Collides(Rectangle bounds)
        {
            throw new NotImplementedException();
        }

        public void Collide()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.EntityTexture, this.Position, BoundingBox, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
