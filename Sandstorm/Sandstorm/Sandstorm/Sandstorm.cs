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
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

namespace Sandstorm
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Sandstorm : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        CameraController cameraController;
        private SpriteBatch SpriteBatch;
        private SpriteFont SpriteFont;

        /// <summary>
        /// Gets or Sets the camera of the scene
        /// </summary>
        public Camera Camera { get; set; }


        private Terrain Terrain;
        private ParticleStorm _particleSystem;
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
            Camera = new Camera(GraphicsDevice.Viewport);
            cameraController = new CameraController(Camera);

            Terrain = new Terrain(this);


            _particleSystem = new ParticleStorm(this);

            //load config
             var obj = LoadXMLConfig("particle.xml", typeof(ParticleProperties));
             _particleSystem.ParticleProperties = obj as ParticleProperties;
             if (_particleSystem.ParticleProperties == null)
             {
                 _particleSystem.ParticleProperties = ParticleProperties.Default;
             }
            //uncommet to create first file
             //StoreXMLConfig("outputconfig.xml", this._particleSystem.ParticleProperties, typeof(ParticleProperties));


            FPSCounter = new FPSCounter(this);

            _HUD = new HUD(this,_particleSystem);

            base.Initialize();
        }

        /// <summary>
        /// called on close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(object sender, EventArgs args)
        {
            //basicly unused... maybe save some special config data
            base.OnExiting(sender, args);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteFont = Content.Load<SpriteFont>("font\\Font");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() { }

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

        /// <summary>
        /// 
        /// </summary>
        private void HandleInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.F11))
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
            if (this.IsActive && !_HUD._gui.HasMouse)
                cameraController.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _particleSystem.UpdateParticles(gameTime);
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            _particleSystem.SetMatrices(Camera.ViewMatrix, Camera.ProjMatrix);
            Terrain.SetMatrices(Camera.ViewMatrix, Camera.ProjMatrix);
            
            base.Draw(gameTime);


            string text = string.Format(CultureInfo.CurrentCulture, "Active Particles: {0}\n", _particleSystem.ActiveParticles);

            SpriteBatch.Begin();

            SpriteBatch.DrawString(SpriteFont, text, new Vector2(10, 25), Color.White);

            SpriteBatch.End();

            _HUD.Draw(gameTime);
        }


        /// <summary>
        /// load config from disc
        /// </summary>
        /// <param name="_filename"></param>
        /// <param name="_type"></param>
        /// <returns></returns>
        private Object LoadXMLConfig(String _filename, Type _type)
        {
            try
            {
                //load ParticleSettings
                XmlSerializer mySerializer = new XmlSerializer(_type);
                StreamReader myReader = new StreamReader(_filename);

                var obj = mySerializer.Deserialize(myReader);
                myReader.Close();

                return obj;
            }
            catch(Exception e)
            {
                Debug.WriteLine("LoadXML", "Something gone wrong while " + _filename + " loading! " + e.Message); 
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
        private bool StoreXMLConfig(String _filename, Object _config, Type _type)
        {
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(_type);
                StreamWriter myWriter = new StreamWriter(_filename);

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
    }
}
