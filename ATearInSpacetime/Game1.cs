using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace ATearInSpacetime
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const int SCREEN_WIDTH = 1024;
        public const int SCREEN_HEIGHT = 768;
        public const int TARGET_WIDTH = 640;
        public const int TARGET_HEIGHT = 480;
        const bool FULL_SCREEN = true;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Game1 instance;
        public static Effect triangleEffect;
        public static Effect postEffect;

        public float timeSinceButtonPress = 1000;

        public float timeSinceExplosion = 1;
        public float timeSinceClump = 1;

        public float timeSinceGameStart = 0;

        float textFade;

        public List<triangleEntity> entities;
        public triangleEntity[] particles = new triangleEntity[30];

        VertexDeclaration vd;

        Vector2 screenShake;

        RenderTarget2D sceneTarget;

        SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            instance = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.IsFullScreen = FULL_SCREEN;
            graphics.ApplyChanges();

            base.Initialize();
        }

        public Song song;
        public SoundEffect charge;
        public SoundEffect clump;
        public SoundEffect explode;
        public SoundEffect bigExplosion;
        public SoundEffect shoot;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            triangleEffect = Content.Load<Effect>("TriangleEffect");
            postEffect = Content.Load<Effect>("Post");
            font = Content.Load<SpriteFont>("font");
            vd = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);
            song = Content.Load<Song>("atearinspacetime");
            charge = Content.Load<SoundEffect>("charge");
            clump = Content.Load<SoundEffect>("clump");
            explode = Content.Load<SoundEffect>("explode");
            bigExplosion = Content.Load<SoundEffect>("bigExplosion");
            shoot = Content.Load<SoundEffect>("shoot");
            Noise3.init();
            
            
            sceneTarget = new RenderTarget2D(GraphicsDevice, TARGET_WIDTH, TARGET_HEIGHT, 1, SurfaceFormat.Bgr32);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(song);

            NewGame();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>n
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                this.Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            timeSinceGameStart += dt;
            timeSinceExplosion += dt;
            timeSinceClump += dt;
            timeSinceButtonPress += dt;

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Update(dt);
                if (entities[i].destroyed)
                {
                    entities.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Update(dt);
            }

            if (gameOver)
            {
                timeSinceGameOver += dt;

                if (timeSinceGameOver > 1.5f)
                {
                    NewGame();
                }
            }
            float moveSpeed = 5.0f;
            float noiseScale = 1.0f;
            screenShake.X = Noise3.noise((timeSinceGameStart * moveSpeed) * noiseScale,
                                  0);

            screenShake.Y = Noise3.noise(0,
                                  (timeSinceGameStart * moveSpeed) * noiseScale);

            float screenShakeScale = 0.00075f;
            if (timeSinceExplosion < 0.5f)
            {
                float flash = 1 - (timeSinceExplosion / 0.5f);
                flash *= flash;
                screenShakeScale += flash * 0.0075f;
            }

            screenShake *= screenShakeScale;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(0, sceneTarget);

            Color clearColor = Color.Black;

            if (timeSinceExplosion < 0.25f)
            {
                float flash = 1 - (timeSinceExplosion/0.25f);
                flash *= flash;
                clearColor = new Color(0, flash * 0.05f, flash * 0.1f);
            }
            else if (timeSinceClump < 0.25f)
            {
                float flash = 1 - (timeSinceClump / 0.25f);
                flash *= flash;
                clearColor = new Color(flash * 0.05f, flash * 0.025f, 0);
            }

            GraphicsDevice.Clear(clearColor);

            GraphicsDevice.VertexDeclaration = vd;

            GraphicsDevice.RenderState.AlphaBlendEnable = true;
            GraphicsDevice.RenderState.AlphaTestEnable = true;
            GraphicsDevice.RenderState.SourceBlend = Blend.One;
            GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceColor;
            GraphicsDevice.RenderState.DepthBufferEnable = false;
            GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

            GraphicsDevice.RenderState.CullMode = CullMode.None;

            triangleEffect.Begin();
            triangleEffect.CurrentTechnique.Passes[0].Begin();

            foreach (triangleEntity entity in entities)
            {
                entity.Draw(GraphicsDevice);
            }

            foreach (triangleEntity entity in particles)
            {
                if (!entity.hidden)
                {
                    entity.Draw(GraphicsDevice);
                }
            }

            triangleEffect.CurrentTechnique.Passes[0].End();

            triangleEffect.CurrentTechnique.Passes[1].Begin();

            foreach (triangleEntity entity in entities)
            {
                entity.Draw2(GraphicsDevice);
            }

            foreach (triangleEntity entity in particles)
            {
                if (!entity.hidden)
                {
                    entity.Draw2(GraphicsDevice);
                }
            }
            triangleEffect.CurrentTechnique.Passes[1].End();
            triangleEffect.End();

            if (timeSinceButtonPress > 30)
            {
                textFade = 1;
            }
            else if (textFade > 0)
            {
                textFade -= (float)gameTime.ElapsedGameTime.TotalSeconds / 5;
            }
            else
            {
                textFade = 0;
            }

            Color textColor = new Color(textFade, textFade, textFade);

            spriteBatch.Begin(SpriteBlendMode.Additive, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            spriteBatch.DrawString(font, "A TEAR IN SPACE-TIME", new Vector2(320, 112), textColor, (float)Math.PI / 2, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "A TEAR IN SPACE-TIME", new Vector2(320, 368), textColor, -(float)Math.PI / 2, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "Controls\nStick: Move\nHold Button 1: Launch Bomb\nHold Button 2: Launch Shard",
                                    new Vector2(260, 112), textColor, (float)Math.PI / 2, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "Controls\nStick: Move\nHold Button 1: Launch Bomb\nHold Button 2: Launch Shard",
                                    new Vector2(390, 368), textColor, -(float)Math.PI / 2, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "Press Btn 4 To Exit", new Vector2(18, 290), new Color(0, 180, 255), (float)Math.PI / 2, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "Press Btn 4 To Exit", new Vector2(622, 190), new Color(0, 180, 255), -(float)Math.PI / 2, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            
            spriteBatch.End();

            //post
            GraphicsDevice.SetRenderTarget(0, null);
            Texture2D sceneTex = sceneTarget.GetTexture();
            postEffect.Parameters["offset"].SetValue(screenShake);
            postEffect.Parameters["t"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            postEffect.CommitChanges();
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            postEffect.Begin();
            postEffect.CurrentTechnique.Passes[0].Begin();
            spriteBatch.Draw(sceneTex, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);
            postEffect.CurrentTechnique.Passes[0].End();
            postEffect.End();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void NewGame()
        {
            entities = new List<triangleEntity>();
            entities.Add(new Ship(new Vector2(-0.75f * (4.0f / 3), 0), -1 * (4.0f / 3), -0.25f * (4.0f / 3), new Vector2(1, 0), new Color(0, 100, 0), Keys.R, Keys.F, Keys.D, Keys.G, Keys.W, Keys.I));
            entities.Add(new Ship(new Vector2(0.75f * (4.0f / 3), 0), 0.25f * (4.0f / 3), 1 * (4.0f / 3), new Vector2(-1, 0), new Color(128, 0, 128), Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.LeftShift, Keys.Z));
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = Particle.RandomParticle();
            }
            
            gameOver = false;
        }


        public static Random rand = new Random();
        public static Vector2 randUnitVector()
        {
            double angle = rand.NextDouble() * Math.PI * 2;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public bool gameOver;
        float timeSinceGameOver;
        public void GameOver(triangleEntity destroyedShip)
        {
            if (!gameOver)
            {
                gameOver = true;
                timeSinceGameOver = 0;
                bigExplosion.Play();
            }
        }
    }
}
