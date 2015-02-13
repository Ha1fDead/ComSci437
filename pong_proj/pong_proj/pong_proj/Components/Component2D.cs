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
        /// <summary>
        /// The texture of the component. Is used to calculate width/height (so the physics match the image)
        /// </summary>
        protected Texture2D EntityTexture;

        /// <summary>
        /// The position of the component, in local coordinates
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The width of the entity, measured from the topleft corner to the topright corner, in local coordinates
        /// </summary>
        protected int width;

        /// <summary>
        /// The height of the entity, measured from the topleft corner to the bottemleft, in local coordinates
        /// </summary>
        protected int height;

        /// <summary>
        /// The velocity in terms of units per second
        /// </summary>
        public Vector2 Velocity;

        /// <summary>
        /// True if the object has collision
        /// </summary>
        public bool collidable;

        /// <summary>
        /// The bounding box, calculated from the position, width, and height
        /// </summary>
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int) Position.X, (int) Position.Y, width, height);
            }
        }

        /// <summary>
        /// Method inherited from XNA. Currently do not use?
        /// </summary>
        public void Initialize()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="collidable"></param>
        public void Initialize(Texture2D texture, Vector2 position, Vector2 velocity, bool collidable)
        {
            this.Position = position;
            this.EntityTexture = texture;
            this.width = this.EntityTexture.Width;
            this.height = this.EntityTexture.Height;
            this.Velocity = velocity;
            this.collidable = collidable;
            this.Initialize();
        }

        /// <summary>
        /// Gets the projected coordinates, in local coordinates, of where this will be in the absolute second
        /// </summary>
        /// <returns></returns>
        public Rectangle GetProjectedCoordinates()
        {
            Rectangle rect = new Rectangle( (int) (this.Position.X+this.Velocity.X), (int) (this.Position.Y+this.Velocity.Y), width, height);
            return rect;
        }

        /// <summary>
        /// Draws the image with the texture and the position
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(EntityTexture, transformToWorld(this.Position), Color.White);
        }

        /// <summary>
        /// Determines if the objects collide
        /// 
        /// If collidable is false, returns false always
        /// </summary>
        /// <param name="bounds">The bounds of the colliding object</param>
        /// <returns></returns>
        public bool Collides(Rectangle bounds)
        {
            //occasionally this will cause clipping, however this is preferably to objects getting "stuck" together
            if (this.collidable && this.GetProjectedCoordinates().Intersects(bounds))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected Vector2 transformToWorld(Vector2 localCoords)
        {
            return new Vector2(localCoords.X, localCoords.Y);
        }

        /// <summary>
        /// Abstract method that executes when two components collide
        /// </summary>
        /// <param name="entityBounds"></param>
        /// <param name="entity"></param>
        public abstract void Collide(Rectangle entityBounds, IEntity2D entity);

        /// <summary>
        /// Abstract method that executes when the components need to update.
        /// 
        /// Will update positions and any other data the implementing classes might need.
        /// </summary>
        /// <param name="gameTime">The gameTime since the last update</param>
        public abstract void Update(GameTime gameTime);
    }
}
