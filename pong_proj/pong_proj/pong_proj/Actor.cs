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
    /// The class containing primary methods for the players or AIs. These include move up and move down, spike, 
    /// </summary>
    class Actor : Entity
    {
        /// <summary>
        /// Up to 4, from PlayerIndex
        /// </summary>
        public PlayerIndex ActorNumber;

        public Actor(Vector2 position, Vector2 velocity, Texture2D texture, PlayerIndex number) : base(position, velocity, texture)
        {
            
        }

        public void UpdateActor(GameTime gameTime)
        {
            ActorNumber = 0;
            this.Update(gameTime);
        }
    }
}
