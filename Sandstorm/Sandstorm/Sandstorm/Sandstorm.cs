using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sandstorm.Terrain;
using Sandstorm.ParticleSystem;


namespace Sandstorm
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Sandstorm : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera _camera;
        CameraController _cameraController;
        HeightMap _heightMap;
        Galaxy _particleSystem;

        public Sandstorm()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 500;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

            _camera = new Camera(GraphicsDevice.Viewport);
            _cameraController = new CameraController(_camera);

            _heightMap = new HeightMap(GraphicsDevice, Content, _camera);
            _particleSystem = new Galaxy(GraphicsDevice, Content, _camera);

            base.Initialize();
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            _camera.Viewport = GraphicsDevice.Viewport;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _heightMap.GenerateHeightField(512, 512);
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
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            if(this.IsActive) //Update only if its in focus
                _cameraController.Update(gameTime);

            _particleSystem.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _heightMap.Draw();
            _particleSystem.Draw();

            base.Draw(gameTime);
        }
    }
}
