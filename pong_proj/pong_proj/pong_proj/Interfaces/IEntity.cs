using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pong_proj
{
    /// <summary>
    /// Class containing metadata for objects, such as ball, bar, wall, etc.
    /// </summary>
    interface IEntity2D : IGameComponent
    {
        /// <summary>
        /// All components contain these three fields, and as such all components should be initialized with these.
        /// </summary>
        /// <param name="texture">The 2d texture of the entity</param>
        /// <param name="position">The 2d position of the object</param>
        /// <param name="velocity">The 2d velocity of the object</param>
        void Initialize(Texture2D texture, Vector2 position, Vector2 velocity, bool collidable);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);

        Rectangle GetProjectedCoordinates(GameTime gameTime);

        bool Collides(GameTime gameTime, Rectangle bounds);

        void Collide(GameTime gameTime, Rectangle entityBounds, IEntity2D entity);
    }
}
