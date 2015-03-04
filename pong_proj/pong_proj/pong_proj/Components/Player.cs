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
    /// Player corresponds to a component (his bar) and has a score.
    /// </summary>
    class Player : Component2D
    {
        /// <summary>
        /// Constant field to set how fast the player moves in their 1d axis
        /// </summary>
        public const int PLAYER_VERTICAL_MOVE_SPEED = 100;

        /// <summary>
        /// Sets the player number, for purposes of controls and gamepads
        /// </summary>
        protected PlayerIndex ActorNumber;

        /// <summary>
        /// Holds the number of times this player has scored
        /// 
        /// The game manager will determine how many points are needed to win
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// Holds the font used to render the player score
        /// 
        /// **TODO** abstract this out to a service or class, such that we can use the same font in multiple classes / locations (menus, effects, etc.)
        /// </summary>
        private SpriteFont _font;

        public Player(PlayerIndex number, SpriteFont font)
        {
            this.ActorNumber = number;
            this.Score = 0;
            this._font = font;
        }

        public override void Collide(GameTime gameTime, Rectangle entityBounds, IEntity2D entity)
        {
            if(entity.GetType().Equals(typeof(Pong)))
            {
                ((Pong)entity).SetLastHitBy(this);
            }
            else if(entity.GetType().Equals(typeof(Wall)))
            {
                this._velocity.Y = 0;
            }
        }

        public override void Update(GameTime gameTime)
        {
            this._position.X += (this._velocity.X * (float)(gameTime.ElapsedGameTime.TotalSeconds));
            this._position.Y += (this._velocity.Y * (float)(gameTime.ElapsedGameTime.TotalSeconds));

            if (isMovingDown())
            {
                this._velocity.Y = PLAYER_VERTICAL_MOVE_SPEED;
            }
            else if (isMovingUp())
            {
                this._velocity.Y = -1 * PLAYER_VERTICAL_MOVE_SPEED;
            }
            else
            {
                this._velocity.Y = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_entityTexture, this.transformToWorld(_position), Color.White);
            spriteBatch.DrawString(_font, this.Score.ToString(), this.transformToWorld(new Vector2(this._position.X, this._position.Y)), Color.LightGreen,
                0, (_font.MeasureString(this.Score.ToString()) / 2), 1.0f, SpriteEffects.None, 0.5f);

        }

        /// <summary>
        /// Function to increment score
        /// </summary>
        public void ScorePoint()
        {
            this.Score++;
        }

        /// <summary>
        /// Returns the vertical velocity in m/s as an integer
        /// </summary>
        /// <returns></returns>
        public int GetVerticalMovement()
        {
            return (int) this._velocity.Y;
        }

        /// <summary>
        /// Abstraction to determine if this player is moving down.
        /// 
        /// Will check various inputs seperately, and handles gamepads dynamically
        /// </summary>
        /// <returns></returns>
        private bool isMovingDown()
        {
            if (Keyboard.GetState(ActorNumber).IsKeyDown(Keys.Down))
            {
                return true;
            }
            if (GamePad.GetState(ActorNumber).IsButtonDown(Buttons.DPadDown))
            {
                return true;
            }
            if (GamePad.GetState(ActorNumber).ThumbSticks.Left.Y < -0.5)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Abstraction to determine if this player is moving up.
        /// 
        /// Will check various inputs seperately, and handles gamepads dynamically
        /// </summary>
        /// <returns></returns>
        private bool isMovingUp()
        {
            if (Keyboard.GetState(ActorNumber).IsKeyDown(Keys.Up))
            {
                return true;
            }
            if(GamePad.GetState(ActorNumber).IsButtonDown(Buttons.DPadUp))
            {
                return true;
            }
            if (GamePad.GetState(ActorNumber).ThumbSticks.Left.Y > 0.5)
            {
                return true;
            }
            return false;
        }

        public override void Reset()
        {
            this.Score = 0;
            base.Reset();
        }
    }
}
