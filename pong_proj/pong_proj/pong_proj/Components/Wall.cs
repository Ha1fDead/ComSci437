using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pong_proj
{
    class Wall : Component2D
    {
        public Wall()
        {

        }

        public override void Collide(Rectangle entityBounds, IEntity2D entity)
        {
            
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //walls don't move
        }
    }
}
