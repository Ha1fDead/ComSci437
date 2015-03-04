using Microsoft.Xna.Framework;
using pong_proj.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pong_proj
{
    class Pong : Component2D
    {
        public const int HIT_VELOCITY_INCREASE = 10;
        public const int PONG_MAX_VELOCITY = 300;

        /// <summary>
        /// Controls who last hit the ball. Should the ball go out of bounds, they get the point.
        /// 
        /// I feel as though this is a better gameplay mechanic (and easier to implement!) than just checking gamebounds
        /// </summary>
        private Player _lastHitBy;

        /// <summary>
        /// Holds the gamebounds, for calculating out-of-boundsism
        /// 
        /// **TODO** Refactor this into several functions, to have a generic amount of players
        /// </summary>
        private Rectangle _gameBounds;

        /// <summary>
        /// Will control the "Spin" of the pong, as per the project req.
        /// </summary>
        private Vector2 _spinAcceleration = new Vector2(0, 0);

        /// <summary>
        /// Holds the previous velocity of the ball, so that we can restore to it
        /// </summary>
        private Vector2 _spinTempVelocity;

        /// <summary>
        /// The game state manager, which will be used to switch the gamestate to SCORE or VICTORY
        /// </summary>
        private GameStateManager _gameState;

        public Pong(Rectangle bounds, GameStateManager gameState)
        {
            this._gameBounds = bounds;
            this._gameState = gameState;
        }

        public override void Collide(GameTime gameTime, Rectangle entityBounds, IEntity2D entity)
        {
            var XProjected = this.BoundingBox;
            XProjected.X += (int)(this._velocity.X);
            var YProjected = this.BoundingBox;
            YProjected.Y += (int)(this._velocity.Y);

            //This is a proper fix
            if(XProjected.Intersects(entityBounds))
            {
                this._velocity.X = this._velocity.X * -1;
            }
            else if(YProjected.Intersects(entityBounds))
            {
                this._velocity.Y = this._velocity.Y * -1;
            }
            else
            {
                XProjected = this.BoundingBox;
                XProjected.X += (int)(this._velocity.X * gameTime.ElapsedGameTime.TotalSeconds);
                YProjected = this.BoundingBox;
                YProjected.Y += (int)(this._velocity.Y * gameTime.ElapsedGameTime.TotalSeconds);

                //This will handle edge cases
                if (XProjected.Intersects(entityBounds) || XProjected.Right == entityBounds.Left || XProjected.Left == entityBounds.Right)
                {
                    this._velocity.X = (this._velocity.X * (-1));
                }
                else
                {
                    this._velocity.Y = (this._velocity.Y * (-1));
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            var project = this._position + this._velocity;

            //ball is not in the playing field
            if(!this.BoundingBox.Intersects(_gameBounds))
            {
                //ball collided with a wall
                if(this._lastHitBy != null)
                {
                    this._lastHitBy.ScorePoint();
                }

                this.Reset();

                return;
            }

            this._position.X += (this._velocity.X * (float)(gameTime.ElapsedGameTime.TotalSeconds));
            this._position.Y += (this._velocity.Y * (float)(gameTime.ElapsedGameTime.TotalSeconds));

            this._velocity.X += (this._spinAcceleration.X * (float)(gameTime.ElapsedGameTime.TotalSeconds));
            this._velocity.Y += (this._spinAcceleration.Y * (float)(gameTime.ElapsedGameTime.TotalSeconds));

            //Check to see if we approach zero, to avoid unnecessary calculations
            if (this._spinTempVelocity.X != 0 && Math.Abs(Math.Abs(this._velocity.X) - Math.Abs(this._spinTempVelocity.X)) < 1)
            {
                this._spinAcceleration.X = 0;
                this._velocity.X = this._spinTempVelocity.X;
                this._spinTempVelocity.X = 0;
            }
            if (this._spinTempVelocity.Y != 0 && Math.Abs(Math.Abs(this._velocity.Y) - Math.Abs(this._spinTempVelocity.Y)) < 1)
            {
                this._spinAcceleration.Y = 0;
                this._velocity.Y = this._spinTempVelocity.Y;
                this._spinTempVelocity.Y = 0;
            }
        }

        /// <summary>
        /// Sets the player for who last hit the ball, and increments the ball velocity (X+Y) to make gameplay faster and more hectic
        /// </summary>
        /// <param name="player"></param>
        public void SetLastHitBy(Player player)
        {
            if (Math.Abs(this._velocity.X)+HIT_VELOCITY_INCREASE < PONG_MAX_VELOCITY)
            {
                if (this._velocity.X < 0)
                {
                    this._velocity.X -= HIT_VELOCITY_INCREASE;
                }
                else
                {
                    this._velocity.X += HIT_VELOCITY_INCREASE;
                }
            }
            if ((Math.Abs(this._velocity.Y) + (HIT_VELOCITY_INCREASE / 2)) < PONG_MAX_VELOCITY)
            {
                if (this._velocity.Y < 0)
                {
                    this._velocity.Y -= HIT_VELOCITY_INCREASE / 2;
                }
                else
                {
                    this._velocity.Y += HIT_VELOCITY_INCREASE / 2;
                }
            }
            this._lastHitBy = player;

            ApplySpin();
        }

        public override void Reset()
        {
            this._spinAcceleration.X = 0;
            this._spinAcceleration.Y = 0;

            Random ran = new Random();
            //randomly switch directions
            if(ran.Next()%2 == 0)
            {
                this._initVelocity.X *= -1;
                this._initVelocity.Y *= -1;
            }

            this._gameState.Transition(GameStateManager.GameState.Score);
            this._lastHitBy = null;

            base.Reset();
        }

        /// <summary>
        /// Function that will apply spin to the ball, when it is hit by the player.
        /// 
        /// The spin is an acceleration vector that will dimminish naturally
        /// </summary>
        private void ApplySpin()
        {
            bool spin = false;
            if(this._lastHitBy.GetVerticalMovement() < 0)
            {
                if(this._velocity.Y > 0)
                {
                    spin = true;
                }
            }
            else if(this._lastHitBy.GetVerticalMovement() > 0)
            {
                if (this._velocity.Y  < 0)
                {
                    spin = true;
                }
            }

            if(spin)
            {
                //invert and provide double the velocity on the spin acceleration
                this._spinAcceleration.Y = this._velocity.Y * -2;
                this._spinTempVelocity.Y = (-1) * this._velocity.Y;
            }
        }
    }
}
