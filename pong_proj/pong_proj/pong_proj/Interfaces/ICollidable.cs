using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pong_proj
{
    interface ICollidable : IEntity2D
    {
        Rectangle CalculateBoundingBox();

        bool Collides(Rectangle bounds);

        void Collide();
    }
}
