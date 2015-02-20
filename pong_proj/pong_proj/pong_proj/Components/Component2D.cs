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
        protected Texture2D _entityTexture;

        /// <summary>
        /// The width of the entity, measured from the topleft corner to the topright corner, in local coordinates
        /// </summary>
        protected int _width;

        /// <summary>
        /// The height of the entity, measured from the topleft corner to the bottemleft, in local coordinates
        /// </summary>
        protected int _height;

        /// <summary>
        /// The position of the component, in local coordinates
        /// </summary>
        protected Vector2 _position;
        protected Vector2 _initPosition;


        /// <summary>
        /// The velocity in terms of units per second
        /// </summary>
        protected Vector2 _velocity;
        protected Vector2 _initVelocity;

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
                return new Rectangle((int) _position.X, (int) _position.Y, _width, _height);
            }
        }

        /// <summary>
        /// Method inherited from XNA. Currently do not use?
        /// </summary>
        public void Initialize()
        {

        }

        /// <summary>
        /// Initializes the component with the given variables.
        /// 
        /// Width and height are initialized to the texture's width and height.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="collidable"></param>
        public void Initialize(Texture2D texture, Vector2 position, Vector2 velocity, bool collidable)
        {
            this._position = position;
            this._entityTexture = texture;
            this._width = this._entityTexture.Width;
            this._height = this._entityTexture.Height;
            this._velocity = velocity;
            this.collidable = collidable;
            this.Initialize();
        }

        /// <summary>
        /// Gets the projected coordinates, in local coordinates, of where this will be in the absolute second
        /// </summary>
        /// <returns></returns>
        public Rectangle GetProjectedCoordinates(GameTime gameTime)
        {
            Rectangle rect = new Rectangle( (int) (this._position.X+(this._velocity.X * gameTime.ElapsedGameTime.TotalSeconds)), (int) (this._position.Y+(this._velocity.Y * gameTime.ElapsedGameTime.TotalSeconds)), _width, _height);
            return rect;
        }

        /// <summary>
        /// Draws the image with the texture and the position
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_entityTexture, transformToWorld(this._position), Color.White);
        }

        /// <summary>
        /// Determines if the objects collide
        /// 
        /// If collidable is false, returns false always
        /// </summary>
        /// <param name="bounds">The bounds of the colliding object</param>
        /// <returns></returns>
        public bool Collides(GameTime gameTime, Rectangle bounds)
        {
            //occasionally this will cause clipping, however this is preferably to objects getting "stuck" together
            if (this.collidable && this.GetProjectedCoordinates(gameTime).Intersects(bounds))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Resets the given component to it's base state
        /// </summary>
        public virtual void Reset()
        {
            this._position = _initPosition;
            this._velocity = _initVelocity;
        }

        /// <summary>
        /// A method that transforms local coordinates into screen coordinates
        /// </summary>
        /// <param name="localCoords"></param>
        /// <returns></returns>
        protected Vector2 transformToWorld(Vector2 localCoords)
        {
            return new Vector2(localCoords.X, localCoords.Y);
        }

        /// <summary>
        /// Abstract method that executes when two components collide
        /// </summary>
        /// <param name="entityBounds"></param>
        /// <param name="entity"></param>
        public abstract void Collide(GameTime gameTime, Rectangle entityBounds, IEntity2D entity);

        /// <summary>
        /// Abstract method that executes when the components need to update.
        /// 
        /// Will update positions and any other data the implementing classes might need.
        /// </summary>
        /// <param name="gameTime">The gameTime since the last update</param>
        public abstract void Update(GameTime gameTime);
    }
}
