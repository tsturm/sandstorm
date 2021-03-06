using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleStormDLL.Meshes;
using ParticleStormDLL.Utils;
using ParticleStormDLL.VertexDeclarations;
using System.Collections.Generic;
using ParticleStormDLL.Forces;
using System.Threading;
using Microsoft.Xna.Framework.Content;


namespace ParticleStormDLL
{
    public class ParticleStorm : DrawableGameComponent
    {
        public ParticleProperties ParticleProperties { get; set; }

        public bool DoDraw { get; set; }
        public int TotalParticles { get; set; }
        public int ActiveParticles { get; private set; }

        public bool DoReset { get; set; }

        protected Randomizer Randomizer;

        protected Matrix ViewMatrix;
        protected Matrix ProjectionMatrix;

        public List<BaseForce> Forces { get; set; }
        protected Vector3 ExternalForces;

        public DoubleTexture Heightmap { get; set; }
        public float HeightScale { get; set; }

        protected BaseMesh FullScreenQuad;
        protected BaseMesh Mesh;

        protected Effect EffectUpdate;
        protected Effect EffectDraw;

        protected int RenderTargetSize;
        protected DoubleTarget PositionsRT;
        protected DoubleTarget VelocitiesRT;
        protected DoubleTarget ColorsRT;
        protected DoubleTarget StartColorsRT;
        protected DoubleTarget EndColorsRT;
        protected DoubleTarget SizesRT;

        private double particles = 0;

        protected ContentManager ContentManager;

       // private Vector4[] fields = new Vector4[2] { new Vector4(60, 0, 10, 2000), new Vector4(10, 0, 60, 2000) };

        private TimeSpan Elapsed;
        private int WaitToEmit;

        protected VertexBuffer InstanceBuffer;

        /// <summary>
        /// Creates an instance of ParticleStorm with default values.
        /// </summary>
        public ParticleStorm(Game game) : base(game)
        {
            //Add ParticleSystem to the game components
            Game.Components.Add(this);

            //Create content manager
            ContentManager = new ResourceContentManager(game.Services, Resource1.ResourceManager);
            ContentManager.RootDirectory = "Resources";

            DoDraw = false;

            //Initialize randomizer
            Randomizer = new Randomizer();

            //Initialize forces list
            Forces = new List<BaseForce>();

            //Set default particle properties
            ParticleProperties = ParticleProperties.Sandstorm;

            //Set default total particles
            TotalParticles = 1024*1024;

            WaitToEmit = ParticleProperties.EmissionRate;

            ExternalForces = Vector3.Zero;

            Forces.Add(new Gravity());
            Forces.Add(new Wind(new Vector3(0.0f, 0.0f, 400.0f)));

            //Set Default view matrix
            ViewMatrix = Matrix.Identity;

            //Set default projection matrix
            ProjectionMatrix = Matrix.Identity;

            HeightScale = 145.0f;

            DoReset = false;
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            GraphicsDevice.DeviceResetting += new EventHandler<EventArgs>(OnDeviceResetting);
            GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(OnDeviceReset);        
        }

        /// <summary>
        /// Called when graphics resources need to be loaded.
        /// </summary>
        protected override void LoadContent()
        {
            EffectDraw = ContentManager.Load<Effect>("ParticleDraw");
            EffectUpdate = ContentManager.Load<Effect>("ParticleUpdate");

            Mesh = new Quad(GraphicsDevice);
            FullScreenQuad = new Quad(GraphicsDevice);

            ParticleProperties.Texture = ContentManager.Load<Texture2D>("StarBurst");
            
            Texture2D TmpMap = Game.Content.Load<Texture2D>("tex/testmap");

            Color[] heightMapData = new Color[TmpMap.Width * TmpMap.Height];
            Vector4[] heightMapData2 = new Vector4[TmpMap.Width * TmpMap.Height];
            TmpMap.GetData(heightMapData);

            for (int i = 0; i < heightMapData.Length; i++)
            {
                heightMapData2[i].X = heightMapData[i].R / 255f;
                heightMapData2[i].Y = heightMapData[i].G / 255f;
                heightMapData2[i].Z = heightMapData[i].B / 255f;
                heightMapData2[i].W = 1.0f;
            }

            Heightmap = new DoubleTexture(GraphicsDevice, 420, 420, false, SurfaceFormat.Vector4);

            Heightmap.TextureA.SetData(heightMapData2);
            Heightmap.TextureB.SetData(heightMapData2);
            GenerateRenderTargets();
        }

        private void GenerateRenderTargets()
        {
            //Calculate the RenderTarget size that is needed to hold all the particles
            RenderTargetSize = (int)Math.Ceiling(Math.Sqrt((double)TotalParticles));
            
            //Initialize the data containers
            VertexInstance[] instances = new VertexInstance[RenderTargetSize * RenderTargetSize];
            Vector4[] positions = new Vector4[RenderTargetSize * RenderTargetSize];
            Vector4[] velocities = new Vector4[RenderTargetSize * RenderTargetSize];
            Vector4[] startColors = new Vector4[RenderTargetSize * RenderTargetSize];
            Vector4[] endColors = new Vector4[RenderTargetSize * RenderTargetSize];
            Vector4[] sizes = new Vector4[RenderTargetSize * RenderTargetSize];

            //Fill the data containers
            for (int x = 0; x < TotalParticles; x++)
            {

                instances[x] = new VertexInstance(x, RenderTargetSize);
                positions[x] = Randomizer.nextVector4(ParticleProperties.PositionMin, ParticleProperties.PositionMax);
                velocities[x] = Randomizer.nextVector4(ParticleProperties.VelocityMin, ParticleProperties.VelocityMax);
                startColors[x] = Randomizer.nextVector4(ParticleProperties.StartColorMin, ParticleProperties.StartColorMax);
                endColors[x] = Randomizer.nextVector4(ParticleProperties.EndColorMin, ParticleProperties.EndColorMax);

                float startSize = Randomizer.nextFloat(ParticleProperties.StartSizeMin, ParticleProperties.StartSizeMax);
                float endSize = Randomizer.nextFloat(ParticleProperties.EndSizeMin, ParticleProperties.EndSizeMax);

                sizes[x] = new Vector4(startSize, startSize, endSize, 1);

                float life = Randomizer.nextFloat(ParticleProperties.LifeTimeMin, ParticleProperties.LifeTimeMax);
                positions[x].W = velocities[x].W = life;
            }

            //Generate and set instance VertexBuffer
            InstanceBuffer = new VertexBuffer(GraphicsDevice, VertexInstance.VertexDeclaration, instances.Length, BufferUsage.WriteOnly);
            InstanceBuffer.SetData(instances);

            //Generate and set positions render target
            PositionsRT = new DoubleTarget(GraphicsDevice, RenderTargetSize, RenderTargetSize, false, SurfaceFormat.Vector4, DepthFormat.None);
            PositionsRT.TargetA.SetData(positions);

            //Generate and set velocities render target
            VelocitiesRT = new DoubleTarget(GraphicsDevice, RenderTargetSize, RenderTargetSize, false, SurfaceFormat.Vector4, DepthFormat.None);
            VelocitiesRT.TargetA.SetData(velocities);

            //Generate and set start colors render target
            ColorsRT = new DoubleTarget(GraphicsDevice, RenderTargetSize, RenderTargetSize, false, SurfaceFormat.Vector4, DepthFormat.None);
            ColorsRT.TargetA.SetData(startColors);

            //Generate and set start colors render target
            StartColorsRT = new DoubleTarget(GraphicsDevice, RenderTargetSize, RenderTargetSize, false, SurfaceFormat.Vector4, DepthFormat.None);
            StartColorsRT.TargetA.SetData(startColors);

            //Generate and set end colors render target
            EndColorsRT = new DoubleTarget(GraphicsDevice, RenderTargetSize, RenderTargetSize, false, SurfaceFormat.Vector4, DepthFormat.None);
            EndColorsRT.TargetA.SetData(endColors);

            //Generate and set end colors render target
            SizesRT = new DoubleTarget(GraphicsDevice, RenderTargetSize, RenderTargetSize, false, SurfaceFormat.Vector4, DepthFormat.None);
            SizesRT.TargetA.SetData(sizes);

            //Ready to draw
            DoDraw = true;
        }

        /// <summary>
        /// Sets the view and projection matrix to draw the scene
        /// </summary>
        /// <param name="viewMatrix">The scenes view matrix</param>
        /// <param name="projectionMatrix">The scenes projection matrix</param>
        public void SetMatrices(Matrix viewMatrix, Matrix projectionMatrix)
        {
            //Set view matrix
            ViewMatrix = viewMatrix;

            //Set projection matrix
            ProjectionMatrix = projectionMatrix;
        }

        /// <summary>
        /// Called when graphics resources need to be unloaded.
        /// </summary>
        protected override void UnloadContent() { }

        /// <summary>
        /// Called after device reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceReset(object sender, EventArgs e)
        {
            PositionsRT.Restore(GraphicsDevice);
            VelocitiesRT.Restore(GraphicsDevice);
            StartColorsRT.Restore(GraphicsDevice);
            EndColorsRT.Restore(GraphicsDevice);
            SizesRT.Restore(GraphicsDevice);
        }

        /// <summary>
        /// Called before device reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnDeviceResetting(object sender, EventArgs e)
        {
            PositionsRT.Backup();
            VelocitiesRT.Backup();
            StartColorsRT.Backup();
            EndColorsRT.Backup();
            SizesRT.Backup();
        }

        /// <summary>
        /// Reset particles
        /// </summary>
        public void Reset()
        {
            DoDraw = false;
            DoReset = false;
            ActiveParticles = 0;
            GenerateRenderTargets();
        }

        /// <summary>
        /// Called when the GameComponent needs to be updated.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update</param>
        public override void Update(GameTime gameTime)
        {
            if (DoDraw)
            {
                particles += (gameTime.ElapsedGameTime.TotalSeconds * ParticleProperties.EmissionRate);
                if (particles >= 1)
                {
                    if (ActiveParticles < TotalParticles) ActiveParticles += (int)Math.Floor(particles);
                    particles = 0;
                }

                //if (ActiveParticles < TotalParticles) ActiveParticles += (int)particles;//(gameTime.TotalGameTime.TotalSeconds * ParticleProperties.EmissionRate);

                //Reset accumulated external forces
                ExternalForces = new Vector3(0.0f, 0.0f, 0.0f);

                //Accumulate all existing external forces
                foreach (var force in Forces)
                {
                    ExternalForces += force.Update(gameTime);
                }

                Heightmap.Swap();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateParticles(GameTime gameTime)
        {
            if (DoDraw)
            {
                try
                {

                    //Set RenderTarget
                    GraphicsDevice.SetRenderTargets(new RenderTargetBinding(PositionsRT.TargetB),
                                                    new RenderTargetBinding(VelocitiesRT.TargetB),
                                                    new RenderTargetBinding(SizesRT.TargetB),
                                                    new RenderTargetBinding(ColorsRT.TargetB));

                    //Clear RenderTarget
                    GraphicsDevice.Clear(Color.Black);

                    //Disable Depth-/Stencilbuffer
                    GraphicsDevice.DepthStencilState = DepthStencilState.None;

                    //Set BlendState
                    GraphicsDevice.BlendState = BlendState.Opaque;

                    //Set VertexSamplerStates
                    GraphicsDevice.VertexSamplerStates[0] = SamplerState.PointClamp;
                    GraphicsDevice.VertexSamplerStates[1] = SamplerState.PointClamp;
                    GraphicsDevice.VertexSamplerStates[2] = SamplerState.PointClamp;
                    GraphicsDevice.VertexSamplerStates[3] = SamplerState.PointClamp;

                    //Set Indexbuffer
                    GraphicsDevice.Indices = FullScreenQuad.IndexBuffer;

                    //Set Vertexbuffer
                    GraphicsDevice.SetVertexBuffer(FullScreenQuad.VertexBuffer);

                    //Set effect technique
                    EffectUpdate.CurrentTechnique = EffectUpdate.Techniques["Update"];

                    //Set effect parameters
                    EffectUpdate.Parameters["Positions"].SetValue(PositionsRT.TargetA);
                    EffectUpdate.Parameters["Velocities"].SetValue(VelocitiesRT.TargetA);
                    EffectUpdate.Parameters["Terrain"].SetValue(Heightmap.TextureA);
                    EffectUpdate.Parameters["Sizes"].SetValue(SizesRT.TargetA);
                    EffectUpdate.Parameters["Colors"].SetValue(ColorsRT.TargetA);
                    EffectUpdate.Parameters["StartColors"].SetValue(StartColorsRT.TargetA);
                    EffectUpdate.Parameters["EndColors"].SetValue(EndColorsRT.TargetA);
                    EffectUpdate.Parameters["ActiveParticles"].SetValue(ActiveParticles);
                    EffectUpdate.Parameters["TotalParticles"].SetValue(TotalParticles);
                    EffectUpdate.Parameters["RenderTargetSize"].SetValue(RenderTargetSize);
                    EffectUpdate.Parameters["ExternalForces"].SetValue(ExternalForces);
                    EffectUpdate.Parameters["PositionMin"].SetValue(ParticleProperties.PositionMin);
                    EffectUpdate.Parameters["PositionMax"].SetValue(ParticleProperties.PositionMax);
                    EffectUpdate.Parameters["LifeMin"].SetValue(ParticleProperties.LifeTimeMin);
                    EffectUpdate.Parameters["LifeMax"].SetValue(ParticleProperties.LifeTimeMax);
                    EffectUpdate.Parameters["StartSizeMin"].SetValue(ParticleProperties.StartSizeMin);
                    EffectUpdate.Parameters["StartSizeMax"].SetValue(ParticleProperties.StartSizeMax);
                    EffectUpdate.Parameters["EndSizeMin"].SetValue(ParticleProperties.EndSizeMin);
                    EffectUpdate.Parameters["EndSizeMax"].SetValue(ParticleProperties.EndSizeMax);
                    EffectUpdate.Parameters["VelocityMin"].SetValue(ParticleProperties.VelocityMin);
                    EffectUpdate.Parameters["VelocityMax"].SetValue(ParticleProperties.VelocityMax);
                    EffectUpdate.Parameters["StartColorMin"].SetValue(ParticleProperties.StartColorMin.ToVector4());
                    EffectUpdate.Parameters["StartColorMax"].SetValue(ParticleProperties.StartColorMax.ToVector4());
                    EffectUpdate.Parameters["EndColorMin"].SetValue(ParticleProperties.StartColorMin.ToVector4());
                    EffectUpdate.Parameters["EndColorMax"].SetValue(ParticleProperties.StartColorMax.ToVector4());
                    EffectUpdate.Parameters["MapWidth"].SetValue(Heightmap.Width);
                    EffectUpdate.Parameters["MapHeight"].SetValue(Heightmap.Height);
                    EffectUpdate.Parameters["HeightScale"].SetValue(HeightScale);
                    EffectUpdate.Parameters["Field"].SetValue(new Vector4(0, 0, 0, 0));
                    EffectUpdate.Parameters["ElapsedTime"].SetValue((float)gameTime.ElapsedGameTime.TotalSeconds);

                    //Begin first effect pass
                    EffectUpdate.CurrentTechnique.Passes[0].Apply();

                    foreach (EffectPass pass in EffectUpdate.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        //Draw full screen quad
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                                             0,
                                                             0,
                                                             FullScreenQuad.VertexCount,
                                                             0,
                                                             FullScreenQuad.PrimitiveCount);
                    }



                    PositionsRT.Swap();
                    VelocitiesRT.Swap();
                    SizesRT.Swap();
                    ColorsRT.Swap();
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine("ObjectDisposed ParticleStorm!" + e);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void DrawParticles(GameTime gameTime)
        {
            if (DoDraw)
            {

                //Set Backbuffer as RenderTarget
                GraphicsDevice.SetRenderTarget(null);

                //Clear Backbuffer
                //GraphicsDevice.Clear(Color.Black);

                //Disable Depth-/Stencilbuffer
                GraphicsDevice.DepthStencilState = DepthStencilState.None;

                //Set BlendState
                GraphicsDevice.BlendState = (ParticleProperties.AdditiveBlending) ? BlendState.Additive : BlendState.Opaque;

                //Set ParticleMesh IndexBuffer
                GraphicsDevice.Indices = Mesh.IndexBuffer;

                //Set ParticleMesh VertexBuffer and InstanceBuffer
                GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(Mesh.VertexBuffer, 0, 0),
                                                new VertexBufferBinding(InstanceBuffer, 0, 1));

                //Set effect technique
                if (ParticleProperties.Texture == null)
                {
                    EffectDraw.CurrentTechnique = EffectDraw.Techniques["Draw"];
                }
                else
                {
                    if (ParticleProperties.Texture.Width == 0)
                        ParticleProperties.Texture = ContentManager.Load<Texture2D>("Simple");
                    EffectDraw.CurrentTechnique = EffectDraw.Techniques["DrawTextured"];
                    EffectDraw.Parameters["diffuseMap"].SetValue(ParticleProperties.Texture);
                }

                EffectDraw.Parameters["ViewMatrix"].SetValue(ViewMatrix);
                EffectDraw.Parameters["projMatrix"].SetValue(ProjectionMatrix);
                EffectDraw.Parameters["Positions"].SetValue(PositionsRT.TargetA);
                EffectDraw.Parameters["Colors"].SetValue(ColorsRT.TargetA);
                EffectDraw.Parameters["Sizes"].SetValue(SizesRT.TargetA);

                //Begin pass
                EffectDraw.CurrentTechnique.Passes[0].Apply();

                //Draw particle instances
                GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList,
                                                       0,
                                                       0,
                                                       Mesh.VertexCount,
                                                       0,
                                                       Mesh.PrimitiveCount,
                                                       ActiveParticles);
            }
        }

        /// <summary>
        /// Called when the DrawableGameComponent needs to be drawn.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            //Set Backbuffer as RenderTarget
            GraphicsDevice.SetRenderTarget(null);

            if (DoReset)
            {
                Reset();
            }


            if (ActiveParticles > 0)
            {
                if (ActiveParticles < (1024 * 1024))
                {
                    DrawParticles(gameTime);
                }
                else
                {
                    DoReset = true;
                }              
            }
            
            base.Draw(gameTime);            
        }


    }
}
