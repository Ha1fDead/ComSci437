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
    class Entity
    {
        
        public Texture2D EntityTexture;
        public Vector2 Position;
        public Vector2 Velocity;

        public Entity(Vector2 position, Vector2 velocity, Texture2D texture)
        {
            this.EntityTexture = texture;
            this.Position = position;
            this.Velocity = velocity;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(EntityTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
