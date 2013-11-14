using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sandstorm.Terrain;
using Sandstorm.ParticleSystem;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SandstormKinect;


namespace Sandstorm
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Sandstorm : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public Camera _perspCamera = null;
        public Camera _orthoCamera = null;
        CameraController _cameraController;
        CameraController _cameraController2;
        HeightMap _heightMap;
        SandstormEditor _editor;
        SandstormBeamer _beamer;
        Galaxy _particleSystem;
        SandstormKinectCore _kinectSystem;
        SandstormKinectEvent eventBuffer = null;
        Stopwatch _stopWatch = new Stopwatch();

        SharedList _sharedList = null;

        private FPSCounter _fpsCounter;

        public Sandstorm(SandstormEditor editor, SandstormBeamer beamer, SandstormKinectCore kinectSystem)
        {
            
            _kinectSystem = kinectSystem;
            _editor = editor;
            _beamer = beamer;
            _editor.TerrainHeightChanged += _editor_TerrainHeight_Changed;
            _editor.TerrainColorChanged += _editor_TerrainColor_Changed;
            _editor.TerrainContoursChanged += _editor_TerrainContour_Changed;
            Mouse.WindowHandle = _editor.Handle;            
            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += new System.EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings); 
            Content.RootDirectory = "Content";
            //this.IsFixedTimeStep = false;

            _fpsCounter = new FPSCounter(this);
            Components.Add(_fpsCounter);

            graphics.SynchronizeWithVerticalRetrace = false;
        }

        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            GraphicsAdapter adapter = null;
            foreach (var item in GraphicsAdapter.Adapters)
            {
                if (item.IsProfileSupported(GraphicsProfile.HiDef))
                {
                    adapter = item;
                }
                else
                {
                    if (adapter == null && item.IsProfileSupported(GraphicsProfile.Reach))
                    {
                        adapter = item;
                    }
                }
            }
            if (adapter == null)
            {
                throw new System.NotSupportedException("None of your graphics cards support XNA.");
            }
            e.GraphicsDeviceInformation.Adapter = adapter;
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

            //_kinectSystem.SandstormKinectDepth += new EventHandler<SandstormKinectEvent>(_kinectSystem_SandstormKinectDepth);
            //_kinectSystem.StartKinect();
       

            graphics.PreferredBackBufferWidth = _editor.panel1.Width;
            graphics.PreferredBackBufferHeight = _editor.panel1.Height;
            graphics.PreferMultiSampling = true;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            _sharedList = SharedList.getInstance(GraphicsDevice);

            _beamer.panel1.Resize += new EventHandler(_beamer_ResizeEnd);
            _editor.panel1.Resize += new EventHandler(_editor_ResizeEnd);



            _orthoCamera = Camera.LoadCamera(Camera.ProjectionType.ORTHOGRAPHIC_PROJECTION, _beamer.panel1.Width, _beamer.panel1.Height);
            _perspCamera = Camera.LoadCamera(Camera.ProjectionType.PERSPECTIVE_PROJECTION, _editor.panel1.Width, _editor.panel1.Height);

            _cameraController2 = new CameraController(_orthoCamera);
            _cameraController = new CameraController(_perspCamera);

            _heightMap = new HeightMap(GraphicsDevice, Content);
            _particleSystem = new Galaxy(GraphicsDevice, _sharedList, Content, _perspCamera, _heightMap);

            base.Initialize();

        }

        void _kinectSystem_SandstormKinectDepth(object sender, SandstormKinectEvent e)
        {
            Console.WriteLine("Event");
            this.eventBuffer = e;
            _heightMap.setData(eventBuffer.DepthImage, eventBuffer.Width, eventBuffer.Height);
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

        void _editor_TerrainHeight_Changed(object sender, TerrainArgs e)
        {
            _heightMap._heightScale = (float) e.Height;
        }

        void _editor_TerrainColor_Changed(object sender, TerrainArgs e)
        {
            _heightMap._color0 = e.Color0;
            _heightMap._color1 = e.Color1;
            _heightMap._color2 = e.Color2;
            _heightMap._color3 = e.Color3;
        }

        void _editor_TerrainContour_Changed(object sender, TerrainArgs e)
        {
            _heightMap._displayContours = e.DisplayContours;
            _heightMap._contourSpacing = e.ContourSpacing;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            _heightMap.GenerateHeightField(420, 420);
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
            {
                _cameraController.Update(gameTime);
            }
            
            _cameraController2.Update(gameTime);

            //_orthoCamera.Left(5.1f);

         //   _particleSystem.Update(gameTime);
                       
            base.Update(gameTime);
        }

        void RenderIt(Camera pCamera,IntPtr pHandle)
        {
            //Clear Screen
            if (pCamera == _perspCamera)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
            }
            else
                GraphicsDevice.Clear(Color.Black);

            //Draw Heightmap
            Texture2D heightMapTexture = _heightMap.Draw(pCamera);

            //Draw Particles
            Texture2D particlesTexture = _particleSystem.Draw(pCamera);

            //Beide Texturen vorhanden, nun wird auf den Screen geschrieben!
            GraphicsDevice.SetRenderTarget(null);



            SpriteBatch _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
                SamplerState.LinearClamp, DepthStencilState.Default,
                RasterizerState.CullNone);
                _spriteBatch.Draw(heightMapTexture, new Vector2(0, 0), Color.White);
                _spriteBatch.Draw(particlesTexture, new Vector2(0, 0), Color.White);
            _spriteBatch.End();

            
            GraphicsDevice.Present(null, null, pHandle);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            RenderIt(_perspCamera, _editor.panel1.Handle);
            RenderIt(_orthoCamera, _beamer.panel1.Handle);
            
            GraphicsDevice.Textures[0] = null;

            //_editor.Particles = _particleSystem.NumberOfParticles;
            _editor.FPS = _fpsCounter.FPS;
            base.Draw(gameTime);
        }
    }
}
