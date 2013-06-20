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

        public Sandstorm(SandstormEditor editor, SandstormBeamer beamer, SandstormKinectCore kinectSystem)
        {
            
            _kinectSystem = kinectSystem;
            _editor = editor;
            _beamer = beamer;
            Mouse.WindowHandle = _editor.Handle;            
            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += new System.EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings); 
            Content.RootDirectory = "Content";
            this.IsFixedTimeStep = false;

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
          /*  GC.RegisterForFullGCNotification(1, 1);
            
            // Start a thread using WaitForFullGCProc.
            Thread startpolling = new Thread(() =>
            {
                while (true)
                {
                    // Check for a notification of an approaching collection.
                    GCNotificationStatus s = GC.WaitForFullGCApproach(1000);
                    if (s == GCNotificationStatus.Succeeded)
                    {
                        //Call event

                        Console.WriteLine("GC is about to begin");
                        GC.Collect();

                    }
                    else if (s == GCNotificationStatus.Canceled)
                    {
                        // Cancelled the Registration
                    }
                    else if (s == GCNotificationStatus.Timeout)
                    {
                        // Timeout occurred.
                    }

                    // Check for a notification of a completed collection.
                    s = GC.WaitForFullGCComplete(1000);
                    if (s == GCNotificationStatus.Succeeded)
                    {
                        //Call event
                        Console.WriteLine("GC has ended");
                        int counter = GC.CollectionCount(2);
                        Console.WriteLine("GC Collected {0} objects", counter);
                    }
                    else if (s == GCNotificationStatus.Canceled)
                    {
                        //Cancelled the registration
                    }
                    else if (s == GCNotificationStatus.Timeout)
                    {
                        // Timeout occurred
                    }

                    Thread.Sleep(500);
                }


            });
            startpolling.Start();*/

            Form xnaWindow = (Form)Control.FromHandle((this.Window.Handle));
            xnaWindow.GotFocus += new EventHandler(xnaWindow_GotFocus);
            _editor.Show();
            _beamer.Show();

            _kinectSystem.SandstormKinectDepth += new EventHandler<SandstormKinectEvent>(_kinectSystem_SandstormKinectDepth);
            _kinectSystem.StartKinect();
       

            graphics.PreferredBackBufferWidth = _editor.panel1.Width;
            graphics.PreferredBackBufferHeight = _editor.panel1.Height;
            graphics.PreferMultiSampling = true;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            _beamer.panel1.Resize += new EventHandler(_beamer_ResizeEnd);
            _editor.panel1.Resize += new EventHandler(_editor_ResizeEnd);


            _cameraController2 = new CameraController(_orthoCamera);

            _heightMap = new HeightMap(GraphicsDevice, Content);

            _particleSystem = new Galaxy(GraphicsDevice, Content, _perspCamera, _heightMap); ;

            _orthoCamera = Camera.LoadCamera(Camera.ProjectionType.ORTHOGRAPHIC_PROJECTION, _beamer.panel1.Width, _beamer.panel1.Height);
            _perspCamera = Camera.LoadCamera(Camera.ProjectionType.PERSPECTIVE_PROJECTION, _editor.panel1.Width, _editor.panel1.Height);

            _cameraController2 = new CameraController(_orthoCamera);
            _cameraController = new CameraController(_perspCamera);

            base.Initialize();
        }

        void _kinectSystem_SandstormKinectDepth(object sender, SandstormKinectEvent e)
        {
            this.eventBuffer = e;
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


            if (_heightMap != null && eventBuffer != null)
            {
                _heightMap.setData(eventBuffer.DepthImage, eventBuffer.Width, eventBuffer.Height);
                eventBuffer = null;
            }

            _cameraController2.Update(gameTime);

            //_orthoCamera.Left(5.1f);

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
            _particleSystem.Draw(_perspCamera);
            GraphicsDevice.Present(null, null, _editor.panel1.Handle);

            GraphicsDevice.Clear(Color.Black);
            _heightMap.Draw(_orthoCamera);
            _particleSystem.Draw(_orthoCamera);
            GraphicsDevice.Present(null, null, _beamer.panel1.Handle);

            GraphicsDevice.Textures[0] = null;
        }
    }
}
