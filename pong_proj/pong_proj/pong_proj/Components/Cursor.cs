using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pong_proj
{
    class Cursor : Component2D
    {
        public Cursor()
        {

        }

        public override void Collide(GameTime gameTime, Rectangle entityBounds, IEntity2D entity)
        {
            //do nothing
        }

        public override void Update(GameTime gameTime)
        {
            Position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        }
    }
}
