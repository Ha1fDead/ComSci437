
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pong_proj.Screens
{
    class GameStateManager
    {
        public enum GameState { Title, Playing, Score, Victory };

        public GameState currentState;

        private double currentStateSeconds;

        public GameStateManager()
        {
            currentStateSeconds = 0;
            currentState = GameState.Title;
        }

        public void Update(GameTime gameTime)
        {
            currentStateSeconds += gameTime.ElapsedGameTime.TotalSeconds;

            //This isn't good. Use OOD instead to reduce code clutter
            //Attempted OOD, found microsoft's GameStateManager
                //too complicated to implement for now, let's use that for the 3D project and use a custom solution for now
            switch(currentState)
            {
                case GameState.Title:
                    break;

                case GameState.Playing:
                    break;

                case GameState.Score:
                    if (currentStateSeconds >= 3)
                    {
                        this.Transition(GameState.Playing);
                    }
                    break;

                case GameState.Victory:
                    if (currentStateSeconds >= 3)
                    {
                        this.Transition(GameState.Title);
                    }
                    break;

                default:
                    break;
            }
        }

        public void Transition(GameState gameState)
        {
            currentState = gameState;
            currentStateSeconds = 0;
        }
    }
}
