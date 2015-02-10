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
    /// Base component class that contains default methods
    /// </summary>
    abstract class Component2D : IEntity2D
    {
        protected Texture2D EntityTexture;
        public Vector2 Position;
        protected int width;
        protected int height;
        public Vector2 Velocity;
        public bool collidable;

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int) Position.X, (int) Position.Y, width, height);
            }
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Initialize(Texture2D texture, Vector2 position, Vector2 velocity, bool collidable)
        {
            this.Position = position;
            this.EntityTexture = texture;
            this.width = this.EntityTexture.Width;
            this.height = this.EntityTexture.Height;
            this.Velocity = velocity;
            this.collidable = collidable;
        }

        public Rectangle GetProjectedCoordinates()
        {
            Rectangle rect = new Rectangle( (int) (this.Position.X+this.Velocity.X), (int) (this.Position.Y+this.Velocity.Y), width, height);
            return rect;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(EntityTexture, Position, Color.White);
        }

        public bool Collides(Rectangle bounds)
        {
            //occasionally this will cause clipping, however this is preferably to objects getting "stuck" together
            if (this.GetProjectedCoordinates().Intersects(bounds))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public abstract void Collide(Rectangle entityBounds);

        public abstract void Update(GameTime gameTime);
    }
}
