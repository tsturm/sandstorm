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


namespace Sandstorm
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Terrain : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private VertexBuffer VertexBuffer;
        private IndexBuffer IndexBuffer;
        private Texture2D HeightMap;
        private Effect Effect;
        protected Matrix ViewMatrix;
        protected Matrix ProjectionMatrix;

        public Terrain(Game game) : base(game)
        {
            // TODO: Construct any child components here
            //Add ParticleSystem to the game components
            Game.Components.Add(this);

            //Set Default view matrix
            ViewMatrix = Matrix.Identity;

            //Set default projection matrix
            ProjectionMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Called when graphics resources need to be loaded.
        /// </summary>
        protected override void LoadContent()
        {

            Effect = Game.Content.Load<Effect>("fx\\terrain");
            HeightMap = Game.Content.Load<Texture2D>("tex/heightmap");

            Color[] heightMapData = new Color[HeightMap.Width * HeightMap.Height];
            Vector4[] heightMapData2 = new Vector4[HeightMap.Width * HeightMap.Height];
            HeightMap.GetData(heightMapData);

            for (int i = 0; i < heightMapData.Length; i++)
            {
                heightMapData2[i].X = heightMapData[i].R / 255f;
                heightMapData2[i].Y = heightMapData[i].G / 255f;
                heightMapData2[i].Z = heightMapData[i].B / 255f;
                heightMapData2[i].W = 1.0f;
            }

            HeightMap = new Texture2D(GraphicsDevice, HeightMap.Width, HeightMap.Height, false, SurfaceFormat.Vector4);

            HeightMap.SetData(heightMapData2);


            int heightOver2 = HeightMap.Height / 2;
            int widthOver2 = HeightMap.Width / 2;

            VertexPositionTexture[] vertices = new VertexPositionTexture[HeightMap.Width * HeightMap.Height];

            for (int y = 0; y < HeightMap.Height; y++)
            {
                for (int x = 0; x < HeightMap.Width; x++)
                {
                    vertices[y * HeightMap.Width + x].Position = new Vector3(-widthOver2 + x, 0f, -heightOver2 + y);
                    vertices[y * HeightMap.Width + x].TextureCoordinate = new Vector2((float)x / (float)HeightMap.Width, (float)y / (float)HeightMap.Height);
                }
            }

            VertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData(vertices);

            int[] indices = new int[(HeightMap.Width - 1) * (HeightMap.Height - 1) * 6];
            int counter = 0;
            for (int z = 0; z < HeightMap.Height - 1; z++)
            {
                for (int x = 0; x < HeightMap.Height - 1; x++)
                {
                    indices[counter++] = x + z * HeightMap.Width;
                    indices[counter++] = (x + 1) + z * HeightMap.Width;
                    indices[counter++] = (x + 1) + (z + 1) * HeightMap.Width;

                    indices[counter++] = (x + 1) + (z + 1) * HeightMap.Width;
                    indices[counter++] = x + (z + 1) * HeightMap.Width;
                    indices[counter++] = x + z * HeightMap.Width;
                }
            }

            IndexBuffer = new IndexBuffer(GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData(indices);
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
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        /// <summary>
        /// Called when the DrawableGameComponent needs to be drawn.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            //Set Backbuffer as RenderTarget
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            GraphicsDevice.SetVertexBuffer(VertexBuffer);
            GraphicsDevice.Indices = IndexBuffer;

            Effect.CurrentTechnique = Effect.Techniques["Terrain2"];
            Effect.Parameters["worldMatrix"].SetValue(Matrix.Identity);
            Effect.Parameters["viewMatrix"].SetValue(ViewMatrix);
            Effect.Parameters["projMatrix"].SetValue(ProjectionMatrix);
            Effect.Parameters["heightMap"].SetValue(HeightMap);
            Effect.Parameters["heightScale"].SetValue(100.0f);

            //Begin pass
            Effect.CurrentTechnique.Passes[0].Apply();

            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VertexBuffer.VertexCount, 0, IndexBuffer.IndexCount / 3);

            base.Draw(gameTime);

        }
    }
}
