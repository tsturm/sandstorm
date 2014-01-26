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
using System.Globalization;

using ParticleStormDLL;
using Sandstorm.GUI;
using SandstormKinect;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Text;
using Sandstorm.Navigation;

namespace Sandstorm
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Sandstorm : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        CameraController cameraController;
        CameraController cameraControllerOrtho;
        private SpriteBatch SpriteBatch;
        private SpriteFont SpriteFont;
        private SandstormKinectCore Kinect;

        /// <summary>
        /// Gets or Sets the camera of the scene
        /// </summary>
        private Camera Camera { get; set; }
        private Camera CameraOrtho { get; set; }

        //reference of ortho / normal
        private Camera ActiveCamera { get; set; }


        private Terrain Terrain;
        private ParticleStorm ParticleSystem;
        private FPSCounter FPSCounter;
        private HUD _HUD;


        public Sandstorm()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

#if DEBUG
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
#else
                IsFixedTimeStep = true;
#endif

            //Show Mouse
            IsMouseVisible = true;

            //Allow Resizing
            Window.AllowUserResizing = true;

            // Subscribe to the game window's ClientSizeChanged event.
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Camera = new Camera(GraphicsDevice.Viewport, ProjectionType.PERSPECTIVE_PROJECTION, "PC");
            cameraController = new CameraController(Camera);

            CameraOrtho = new Camera(GraphicsDevice.Viewport,ProjectionType.ORTHOGRAPHIC_PROJECTION,"Beamer");
            CameraOrtho.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), (float)Math.PI / 2);
            cameraControllerOrtho = new CameraController(CameraOrtho);

            ActiveCamera = Camera;

            Terrain = new Terrain(this);
            ParticleSystem = new ParticleStorm(this);
            Kinect = new SandstormKinectCore();

            FPSCounter = new FPSCounter(this);
            _HUD = new HUD(this);

            //load configs
            this.LoadEverythingFromXML();

            //register even handler 
            Kinect.SandstormKinectDepth +=new EventHandler<SandstormKinectEvent>(Handlekinect);
            Kinect.StartKinect();
            
            //init GUI
            _HUD.InitGui();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteFont = Content.Load<SpriteFont>("font\\Font");

            _HUD.AddSubMenu(ParticleSystem.ParticleProperties);
            _HUD.AddSubMenu(Terrain.TerrainProperties);
            _HUD.AddSubMenu(Kinect.KinectSettings);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            Kinect.Exit();
        }

        /// <summary>
        /// Handles Window resizing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnResize(object sender, EventArgs e)
        {
            if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0)
            {
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                Camera.Viewport = new Viewport(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);
            }
            _HUD.OnResize();
        }

        private void Handlekinect(object sender, SandstormKinectEvent e)
        {
            if (    Terrain.HeightMap != null && ParticleSystem.Heightmap != null 
                && !Terrain.HeightMap.DoSwap && !ParticleSystem.Heightmap.DoSwap
                )
            {
                /*Vector4[] tmp = (Vector4[]) e.TextureData.Clone();
                Vector4[] tmp2 = (Vector4[]) e.TextureData.Clone();
                ParticleSystem.Heightmap.TextureB.SetData(tmp);
                Terrain.HeightMap.TextureB.SetData(tmp2);*/
                ParticleSystem.Heightmap.TextureB.SetData(e.TextureData);
                Terrain.HeightMap.TextureB.SetData(e.TextureData);
                ParticleSystem.Heightmap.DoSwap = true;
                Terrain.HeightMap.DoSwap = true;
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        KeyboardState oldState = Keyboard.GetState();
        private void HandleInput()
        {
            MouseState mouseState = Mouse.GetState();

            if (mouseState.X > Window.ClientBounds.Width - (_HUD.MenuOffset - 140))
            {
                _HUD.Show();
            }
            else if (mouseState.X < Window.ClientBounds.Width - _HUD.MenuOffset)
            {
                _HUD.Hide();
            }

            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            else if (newState.IsKeyDown(Keys.F11))
            {
                if (!oldState.IsKeyDown(Keys.F11))
                {
                    if (graphics.IsFullScreen)
                    {
                        graphics.PreferredBackBufferWidth = 800;
                        graphics.PreferredBackBufferHeight = 480;
                        graphics.ToggleFullScreen();
                    }
                    else
                    {
                        graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                        graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                        graphics.ToggleFullScreen();
                    }
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                if (!oldState.IsKeyDown(Keys.C))
                {
                    if (ActiveCamera == Camera)
                        ActiveCamera = CameraOrtho;
                    else
                        ActiveCamera = Camera;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (!oldState.IsKeyDown(Keys.S))
                {
                    this.StoreEverythingToXML();
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                if (!oldState.IsKeyDown(Keys.L))
                {
                    this.LoadEverythingFromXML();
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                if (!oldState.IsKeyDown(Keys.R))
                {
                    ParticleSystem.DoReset = true;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                if (!oldState.IsKeyDown(Keys.H))
                {
                    _HUD.MenuOffset = (_HUD.MenuOffset == 170) ? 340 : 170;
                    _HUD.InitGui();
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                if (!oldState.IsKeyDown(Keys.P))
                {
                    ParticleSystem.DoDraw = !ParticleSystem.DoDraw;
                }
            }

            // Update saved state.
            oldState = newState;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Handle user input
            HandleInput();
            _HUD.Update(gameTime);

            //Mouse camera Events only on focus and if not clicked on buttons
            if (this.IsActive && !_HUD.GUI.HasMouse)
            {
                if(ActiveCamera == CameraOrtho)
                    cameraControllerOrtho.Update(gameTime);
                else if(ActiveCamera == Camera)
                    cameraController.Update(gameTime);
                
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            try
            {
                ParticleSystem.UpdateParticles(gameTime);
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Black);

                ParticleSystem.SetMatrices(ActiveCamera.CameraSettings.ViewMatrix, ActiveCamera.CameraSettings.ProjectionMatrix);
                Terrain.SetMatrices(ActiveCamera.CameraSettings.ViewMatrix, ActiveCamera.CameraSettings.ProjectionMatrix);

                base.Draw(gameTime);


                string text = string.Format(CultureInfo.CurrentCulture, "Active Particles: {0}\nProjectionType: {1}\nCameraName: {2}\nCameraMode: {3}", ParticleSystem.ActiveParticles, ActiveCamera.CameraSettings.ProjectionType, ActiveCamera.CameraSettings.CameraName, ActiveCamera.CameraSettings.CameraMode);

                SpriteBatch.Begin();

                SpriteBatch.DrawString(SpriteFont, text, new Vector2(10, 25), Color.White);

                SpriteBatch.End();

                _HUD.Draw(gameTime);

            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("InvalidOperationException Sandstorm " + e);
            }
            //base.Draw(gameTime);
        }


        #region config handling

        /// <summary>
        /// Store all Properties in single files
        /// </summary>
        private void StoreEverythingToXML()
        {
            StoreXMLConfig(this.ParticleSystem.ParticleProperties);
            StoreXMLConfig(this.Kinect.KinectSettings);
            StoreXMLConfig(this.Terrain.TerrainProperties);
            StoreXMLConfig(this.CameraOrtho.CameraSettings);

            Debug.WriteLine("StoreXML", "All XML Files written!");
        }

        /// <summary>
        /// Load all Properties from single files
        /// </summary>
        private void LoadEverythingFromXML()
        {
            Object obj;

            obj = LoadXMLConfig(typeof(ParticleProperties));
            this.ParticleSystem.ParticleProperties = obj as ParticleProperties;
            if (this.ParticleSystem.ParticleProperties == null)
            {
                ParticleSystem.ParticleProperties = ParticleProperties.Sandstorm;
            }

            obj = LoadXMLConfig(typeof(KinectProperties));
            this.Kinect.KinectSettings = obj as KinectProperties;
            if (this.Kinect.KinectSettings == null)
            {
                this.Kinect.KinectSettings = KinectProperties.Sandstorm;
            }

            obj = LoadXMLConfig(typeof(TerrainProperties));
            this.Terrain.TerrainProperties = obj as TerrainProperties;
            if (this.Terrain.TerrainProperties == null)
            {
                this.Terrain.TerrainProperties = TerrainProperties.Default;
            }

            obj = LoadXMLConfig(typeof(CameraProperties));
            this.CameraOrtho.CameraSettings = obj as CameraProperties;
            if (this.CameraOrtho.CameraSettings == null)
            {
                this.CameraOrtho.CameraSettings = CameraProperties.DefaultOrtho;
                this.CameraOrtho.UpdateViewMatrix();
                this.CameraOrtho.UpdateProjectionMatrix();
            }
            _HUD.InitGui();
            Debug.WriteLine("LoadXML", "All XML Files read!");
        }

        /// <summary>
        /// load config from disc
        /// </summary>
        /// <param name="_filename"></param>
        /// <param name="_type"></param>
        /// <returns></returns>
        internal Object LoadXMLConfig(Type _type)
        {
            try
            {
                //load ParticleSettings
                XmlSerializer mySerializer = new XmlSerializer(_type);
                StreamReader myReader = new StreamReader(_type.Name + ".xml");

                var obj = mySerializer.Deserialize(myReader);
                myReader.Close();

                return obj;
            }
            catch (Exception e)
            {
                Debug.WriteLine("LoadXML", "Something gone wrong while " + _type.Name + ".xml" + " loading! " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// write config to disc
        /// </summary>
        /// <param name="_filename"></param>
        /// <param name="_config"></param>
        /// <param name="_type"></param>
        /// <returns></returns>
        internal bool StoreXMLConfig(Object _config)
        {
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(_config.GetType());
                StreamWriter myWriter = new StreamWriter(_config.GetType().Name + ".xml");

                mySerializer.Serialize(myWriter, _config);
                myWriter.Close();

                return true;

            }
            catch (Exception e)
            {
                Debug.WriteLine("StoreXML", "Something gone wrong while XML storing! " + e.Message);
                return false;
            }

        }

        #endregion
    }
}