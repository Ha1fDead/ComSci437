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

namespace pong_proj
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Space_Pong : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background;
        SpriteFont Font;

        List<IEntity2D> entities;
        List<Player> players;
        Pong pong;

        //This is less than ideal. Ideally we would have world coordinates and board coordinates
        public const int SCREEN_WIDTH = 640;
        public const int SCREEN_HEIGHT = 480;

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
            // TODO: Add your initialization logic here
            this.graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            this.graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            this.graphics.ApplyChanges();

            entities = new List<IEntity2D>();
            players = new List<Player>();

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

            Vector2 player1pos = new Vector2(0, 0);
            Vector2 player2pos = new Vector2(SCREEN_WIDTH - default_texture.Width, 0);

            var player1 = new Player(PlayerIndex.One);
            player1.Initialize(this.Content.Load<Texture2D>("wall"), player1pos, new Vector2(0f, 0f), true);
            entities.Add(player1);
            players.Add(player1);

            var player2 = new Player(PlayerIndex.Two);
            player2.Initialize(this.Content.Load<Texture2D>("wall"), player2pos, new Vector2(0f, 0f), true);
            entities.Add(player2);
            players.Add(player2);

            var pong = new Pong();
            pong.Initialize(this.Content.Load<Texture2D>("RubberBall"), new Vector2(150, 150), new Vector2(2f, 2f), true);

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

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            foreach (var entity in entities)
            {
                //this is REALLY, REALLY DIRTY!!!!
                    //but I'm lazy
                var entityProjectedCoords = entity.GetProjectedCoordinates();

                foreach (var nearbyEntity in getNearbyObjects(entity))
                {
                    var nearbyProjectedCoords = nearbyEntity.GetProjectedCoordinates();

                    if (entity != nearbyEntity && entity.Collides(nearbyProjectedCoords))
                    {
                        entity.Collide(nearbyProjectedCoords);
                        nearbyEntity.Collide(entityProjectedCoords);
                        entityProjectedCoords = entity.GetProjectedCoordinates();
                    }
                }

                entity.Update(gameTime);
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

            //run through every entity and draw them
            foreach (var entity in this.entities)
            {
                entity.Draw(spriteBatch);
            }

            foreach(var player in players)
            {
                string score = player.Score.ToString();
                spriteBatch.DrawString(Font, score, new Vector2(player.Position.X + 30, player.Position.Y + 40), Color.LightGreen,
                    0, (Font.MeasureString(score) / 2), 1.0f, SpriteEffects.None, 0.5f);
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
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
    }
}
