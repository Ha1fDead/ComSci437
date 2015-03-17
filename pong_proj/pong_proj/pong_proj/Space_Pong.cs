using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using pong_proj.Screens;
using Microsoft.Kinect;

namespace pong_proj
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Space_Pong : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GameStateManager state;
        SpriteBatch spriteBatch;
        Texture2D background;
        SpriteFont Font;

        KinectControls kinectControls;

        List<IEntity2D> entities;
        List<Player> players;
        Pong pong;

        //This is less than ideal. Ideally we would have world coordinates and board coordinates
        public const int SCREEN_WIDTH = 640;
        public const int SCREEN_HEIGHT = 480;
        public const int GAME_WIDTH_UNITS = 640;
        public const int GAME_HEIGHT_UNITS = 480;
        public const int GAME_LENGTH = 7;

        public Space_Pong()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            this.graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            this.graphics.ApplyChanges();

            entities = new List<IEntity2D>();
            players = new List<Player>();
            state = new GameStateManager();

            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinectControls = new KinectControls(this);
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {   
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            background = this.Content.Load<Texture2D>("water_background");

            Font = Content.Load<SpriteFont>("Courier New");

            var default_texture = this.Content.Load<Texture2D>("GameThumbnail");
            var horizontalTexture = this.Content.Load<Texture2D>("horizontal_wall");
            var playerTexture = this.Content.Load<Texture2D>("paddle_green");

            Vector2 player1pos = new Vector2(playerTexture.Width*2, horizontalTexture.Height);
            Vector2 player2pos = new Vector2(SCREEN_WIDTH - (playerTexture.Width*3), horizontalTexture.Height);

            var player1 = new Player(PlayerIndex.One, Font, kinectControls);
            player1.Initialize(playerTexture, player1pos, new Vector2(0f, 0f), true);
            entities.Add(player1);
            players.Add(player1);

            var player2 = new Player(PlayerIndex.Two, Font, kinectControls);
            player2.Initialize(playerTexture, player2pos, new Vector2(0f, 0f), true);
            entities.Add(player2);
            players.Add(player2);

            var bottomWall = new Wall();
            bottomWall.Initialize(horizontalTexture, new Vector2(0, 0), new Vector2(0f, 0f), true);
            entities.Add(bottomWall);

            var topWall = new Wall();
            topWall.Initialize(horizontalTexture, new Vector2(0, SCREEN_HEIGHT - horizontalTexture.Height), new Vector2(0f, 0f), true);
            entities.Add(topWall);

            pong = new Pong(background.Bounds, state);

            Random ran = new Random();
            int randomIndex = ran.Next(players.Count);
            if(randomIndex == 0)
            {
                pong.Initialize(this.Content.Load<Texture2D>("RubberBall"), new Vector2(150, 150), new Vector2(40f, 40f), true);
            }
            else
            {
                pong.Initialize(this.Content.Load<Texture2D>("RubberBall"), new Vector2(150, 150), new Vector2(-40f, -40f), true);
            }

            pong.SetLastHitBy(players.ElementAt(randomIndex));

            entities.Add(pong);

            var cursor = new Cursor();
            cursor.Initialize(default_texture, player1pos, new Vector2(), false);
            entities.Add(cursor);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            if(kinectControls != null)
            {
                kinectControls.Unload();
            }
            entities.Clear();
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            state.Update(gameTime);
            if(kinectControls != null)
            {
                kinectControls.Update(gameTime);
            }

            //Bad design, use OOD / Microsoft's GameStateManager code (which is awesome and well thought-out)
            switch (state.currentState)
            {
                case GameStateManager.GameState.Title:
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                    {
                        state.Transition(GameStateManager.GameState.Playing);
                    }
                    break;

                case GameStateManager.GameState.Playing:
                    UpdatePhysics(gameTime);
                    break;

                case GameStateManager.GameState.Score:
                    //update players, nothing else
                    foreach(var player in players)
                    {
                        player.Update(gameTime);
                    }
                    break;

                case GameStateManager.GameState.Victory:
                    break;

                default:
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            this.spriteBatch.Begin();

            //draw background
            this.spriteBatch.Draw(background, new Rectangle(0, 0, 640, 480), Color.White);
            bool drawEntities = false;


            switch (state.currentState)
            {
                case GameStateManager.GameState.Title:
                    break;

                case GameStateManager.GameState.Playing:
                    drawEntities = true;
                    break;

                case GameStateManager.GameState.Score:
                    drawEntities = true;
                    break;

                case GameStateManager.GameState.Victory:
                    break;

                default:
                    break;
            }

            if(drawEntities)
            {
                foreach (var entity in this.entities)
                {
                    entity.Draw(spriteBatch);
                }

                foreach (var player in players)
                {
                    if (player.Score == 7)
                    {
                        foreach (var component in entities)
                        {
                            component.Reset();
                        }
                    }
                }
            }

            if(kinectControls != null)
            {
                kinectControls.Draw(gameTime, spriteBatch);
            }

            base.Draw(gameTime);

            this.spriteBatch.End();
        }

        /// <summary>
        /// Will eventually be a speedup of the current N^2 solution
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private List<IEntity2D> getNearbyObjects(IEntity2D obj)
        {
            return entities;
        }

        private void UpdatePhysics(GameTime gameTime)
        {
            foreach (var entity in entities)
            {
                //this is REALLY, REALLY DIRTY!!!!
                //but I'm lazy, and my 2d physics engine is sufficient for this complex of a game
                var entityProjectedCoords = entity.GetProjectedCoordinates(gameTime);

                foreach (var nearbyEntity in getNearbyObjects(entity))
                {
                    var nearbyProjectedCoords = nearbyEntity.GetProjectedCoordinates(gameTime);

                    if (entity != nearbyEntity && entity.Collides(gameTime, nearbyProjectedCoords))
                    {
                        entity.Collide(gameTime, nearbyProjectedCoords, nearbyEntity);
                        nearbyEntity.Collide(gameTime, entityProjectedCoords, nearbyEntity);
                        entityProjectedCoords = entity.GetProjectedCoordinates(gameTime);
                    }
                }

                entity.Update(gameTime);
            }
        }
    }
}
