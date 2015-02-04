using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pong_proj
{
    class Player : Component2D
    {
        protected PlayerIndex ActorNumber;

        public Player(PlayerIndex number)
        {
            this.ActorNumber = number;
        }
    }
}
