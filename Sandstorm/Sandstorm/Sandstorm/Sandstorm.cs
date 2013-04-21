using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sandstorm.Terrain;
using Sandstorm.ParticleSystem;
using System.Windows.Forms;
using System.Diagnostics;


namespace Sandstorm
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Sandstorm : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera _perspCamera;
        Camera _orthoCamera;
        CameraController _cameraController;
        HeightMap _heightMap;
        SandstormEditor _editor;
        SandstormBeamer _beamer;
        Galaxy _particleSystem;

        public Sandstorm(SandstormEditor editor, SandstormBeamer beamer)
        {
            _editor = editor;
            _beamer = beamer;
            Mouse.WindowHandle = _editor.Handle;
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
            Form xnaWindow = (Form)Control.FromHandle((this.Window.Handle));
            xnaWindow.GotFocus += new EventHandler(xnaWindow_GotFocus);
            _editor.Show();
            _beamer.Show();

            graphics.PreferredBackBufferWidth = _editor.panel1.Width;
            graphics.PreferredBackBufferHeight = _editor.panel1.Height;
            graphics.PreferMultiSampling = true;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            _beamer.panel1.Resize += new EventHandler(_beamer_ResizeEnd);
            _editor.panel1.Resize += new EventHandler(_editor_ResizeEnd);

            // Create perspective camera for the editor
            _perspCamera = new Camera(new Viewport(0, 0, _editor.panel1.Width, _editor.panel1.Height));
            _perspCamera.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.PiOver4);
            _cameraController = new CameraController(_perspCamera);

            // Create orthographic camera for the beamer
            _orthoCamera = new Camera(new Viewport(0, 0, _beamer.panel1.Width, _beamer.panel1.Height));
            _orthoCamera.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.PiOver2);
            _orthoCamera.Type = Camera.ProjectionType.ORTHOGRAPHIC_PROJECTION;
            _particleSystem = new Galaxy(GraphicsDevice, Content, _perspCamera);

            _heightMap = new HeightMap(GraphicsDevice, Content);

            base.Initialize();
        }

        void xnaWindow_GotFocus(object sender, EventArgs e)
        {
            ((Form)sender).Visible = false;
            _editor.TopMost = false;
        }

        void _beamer_ResizeEnd(object sender, EventArgs e)
        {
            if (_beamer.panel1.Width > GraphicsDevice.Viewport.Width || 
                _beamer.panel1.Height > GraphicsDevice.Viewport.Height)
            {
                graphics.PreferredBackBufferWidth = _beamer.panel1.Width;
                graphics.PreferredBackBufferHeight = _beamer.panel1.Height;
                graphics.ApplyChanges();
            }

            _orthoCamera.Viewport = new Viewport(0, 0, _beamer.panel1.Width, _beamer.panel1.Height);
        }

        void _editor_ResizeEnd(object sender, EventArgs e)
        {
            if (_editor.panel1.Width > GraphicsDevice.Viewport.Width || 
                _editor.panel1.Height > GraphicsDevice.Viewport.Height)
            {
                graphics.PreferredBackBufferWidth = _editor.panel1.Width;
                graphics.PreferredBackBufferHeight = _editor.panel1.Height;
                graphics.ApplyChanges();
            }

            _perspCamera.Viewport = new Viewport(0, 0, _editor.panel1.Width, _editor.panel1.Height);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            if (_editor.panel1.Focused)
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
            _heightMap.Draw(_perspCamera);
            //_particleSystem.Draw();
            GraphicsDevice.Present(null, null, _editor.panel1.Handle);

            GraphicsDevice.Clear(Color.CornflowerBlue);
            _heightMap.Draw(_orthoCamera);
            GraphicsDevice.Present(null, null, _beamer.panel1.Handle);
        }
    }
}
