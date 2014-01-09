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

        public int TotalParticles { get; set; }
        public int ActiveParticles { get; private set; }
        public int EmissionRate { get; set; }

        protected Randomizer Randomizer;

        protected Matrix ViewMatrix;
        protected Matrix ProjectionMatrix;

        public List<BaseForce> Forces { get; set; }
        protected Vector3 ExternalForces;

        public Texture2D terrain;

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

            //Initialize randomizer
            Randomizer = new Randomizer();

            //Initialize forces list
            Forces = new List<BaseForce>();

            //Set default particle properties
            //ParticleProperties = ParticleProperties.Default;

            //Set default total particles
            TotalParticles = 1024*1024;

            //Set default emission rate
            EmissionRate = 1075;

            WaitToEmit = EmissionRate;

            ExternalForces = Vector3.Zero;

            Forces.Add(new Gravity());
            //Forces.Add(new Wind(new Vector3(1.0f, 0.5f, 0.2f), 12.0f));

            terrain = Game.Content.Load<Texture2D>("tex/heightmap");

            //Set Default view matrix
            ViewMatrix = Matrix.Identity;

            //Set default projection matrix
            ProjectionMatrix = Matrix.Identity;
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

            ParticleProperties.Texture = ContentManager.Load<Texture2D>("Simple");

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
        /// Called when the GameComponent needs to be updated.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update</param>
        public override void Update(GameTime gameTime)
        {
            //Elapsed += gameTime.ElapsedGameTime;

            //if (Elapsed >= TimeSpan.FromSeconds(1))
            //{
            //    Elapsed = TimeSpan.FromSeconds(0);
            //    if (ActiveParticles + EmissionRate < TotalParticles)
            //    {
            //        WaitToEmit += EmissionRate;
            //    }
            //    else
            //    {
            //        WaitToEmit = TotalParticles - ActiveParticles;
            //    }
            //}

            //if (WaitToEmit > 0)
            //{
            //    ActiveParticles = WaitToEmit;
            //}

            if (ActiveParticles < TotalParticles) ActiveParticles = (int)(gameTime.TotalGameTime.TotalSeconds * EmissionRate);

            //Reset accumulated external forces
            ExternalForces = new Vector3(0.0f, 0.0f, 0.0f);

            //Accumulate all existing external forces
            foreach (var force in Forces)
            {
                ExternalForces += force.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateParticles(GameTime gameTime)
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
            EffectUpdate.Parameters["Terrain"].SetValue(terrain);
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
            EffectUpdate.Parameters["StartColorMin"].SetValue(ParticleProperties.StartColorMin.ToVector4());
            EffectUpdate.Parameters["StartColorMax"].SetValue(ParticleProperties.StartColorMax.ToVector4());
            EffectUpdate.Parameters["EndColorMin"].SetValue(ParticleProperties.StartColorMin.ToVector4());
            EffectUpdate.Parameters["EndColorMax"].SetValue(ParticleProperties.StartColorMax.ToVector4());
            EffectUpdate.Parameters["Field"].SetValue(new Vector4(0, 0, 0, 0));
            EffectUpdate.Parameters["ElapsedTime"].SetValue((float)gameTime.ElapsedGameTime.TotalSeconds);

            //Begin first effect pass
            EffectUpdate.CurrentTechnique.Passes[0].Apply();

            //Draw full screen quad
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 
                                                 0, 
                                                 0, 
                                                 FullScreenQuad.VertexCount, 
                                                 0, 
                                                 FullScreenQuad.PrimitiveCount);

            PositionsRT.Swap();
            VelocitiesRT.Swap();
            SizesRT.Swap();
            ColorsRT.Swap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void DrawParticles(GameTime gameTime)
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

        /// <summary>
        /// Called when the DrawableGameComponent needs to be drawn.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            //Set Backbuffer as RenderTarget
            GraphicsDevice.SetRenderTarget(null);

            if (ActiveParticles > 0 && ActiveParticles < (1024*1024))
            {              
                DrawParticles(gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
