using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Sandstorm.ParticleSystem.structs;
using Sandstorm.Terrain;
using Sandstorm.ParticleSystem.structs.Forces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Sandstorm.ParticleSystem.physic
{
    class PhysicEngine
    {
        private SharedList _sharedList = null;
        private List<Force> _forces = new List<Force>();

        private HeightMap _heightMap;

        private CollisionDetector _collisionDetector;

        private GraphicsDevice _graphicsDevice = null;
        private ContentManager _contentManager = null;
        private VertexBuffer _vertexBuffer = null;
        private Effect _effect;

        private IndexBuffer _indexBuffer = null;
        private DynamicVertexBuffer _instanceVertexBuffer = null;

        private static VertexDeclaration _instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
        );

        public PhysicEngine(GraphicsDevice pGraphicsDevice, ContentManager pContentManager, SharedList pList, HeightMap heightMap)
        {
            this._graphicsDevice = pGraphicsDevice;
            this._contentManager = pContentManager;
            this._sharedList = pList;
            this._heightMap = heightMap;
            _collisionDetector = new CollisionDetector(pList, _heightMap);

            this._forces.Add(new Gravity());
            //this._forces.Add(new Force(new Vector3(-0.00f, -0.0f, 0.1f)));
            this._forces.Add(new Wind(new Vector3(-0.0f, -0.0f, -0.1f), heightMap));



            Vector2[] iVertex = new Vector2[SharedList.SquareSize * SharedList.SquareSize]; //Position auf 

            for (int x = 0; x < SharedList.SquareSize; x++)
                for (int y = 0; y < SharedList.SquareSize; y++)
                {
                    iVertex[x * SharedList.SquareSize + y].X = (float)(x) / SharedList.SquareSize;
                    iVertex[x * SharedList.SquareSize + y].Y = (float)(y) / SharedList.SquareSize;
                }


            InitInstanceVertexBuffer(iVertex);

            _effect = _contentManager.Load<Effect>("fx/Physik");
        }

        void InitInstanceVertexBuffer(Vector2[] pPositions)
        {
            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((_vertexBuffer == null))
            {
                if (_vertexBuffer != null)
                    _vertexBuffer.Dispose();

                _vertexBuffer = new VertexBuffer(_graphicsDevice, _instanceVertexDeclaration,
                                                               pPositions.Length, BufferUsage.None);
            }

            _vertexBuffer.SetData(pPositions);
        }


        public void Update(GameTime pGameTime) //Update physic
        {

            /*        moveParticles();
                    _collisionDetector.checkCollisions();
                    applyForces();*/

        }

        private void moveParticles()
        {
            Parallel.ForEach(_sharedList.getParticles(), p =>
            {
                if (p != null)
                {
                    p.move();
                }
            });
        }

        private void applyForces()
        {
            Parallel.ForEach(_sharedList.getParticles(), p =>
            {
                if (p != null)
                {
                    foreach (Force f in _forces)//apply external forces (Gravitation etc)
                    {
                        f.apply_to(p);
                    }
                }
            });
        }


        double KreisPos = 0;
        int offset = 0;
        public void Draw() //Nothing to draw.. normally
        {
            /*Texture2D pos = _sharedList.ParticlePositions;
            Vector4[] data = new Vector4[SharedList.SquareSize * SharedList.SquareSize];
            pos.GetData<Vector4>(data);

            for (int i = 0; i < SharedList.SquareSize; i++)
                for (int j = 0; j < SharedList.SquareSize; j++)
                {
                    int index = i * SharedList.SquareSize + j;
                    //data[i].X = ;
                    //data[i].Y += 1.0f;
                    data[index].Y = (float)(50.00f * Math.Sin(((j + offset) % SharedList.SquareSize )* KreisPos));
                }
           
            KreisPos = 0.2f;
            offset += 1;
            pos.SetData(data);*/



            //  _graphicsDevice.SetRenderTarget(renderTarget1);

            //  _graphicsDevice.Clear(Color.Transparent);

            RenderTarget2D rt = new RenderTarget2D(_graphicsDevice, SharedList.SquareSize, SharedList.SquareSize, false, SurfaceFormat.Vector4, DepthFormat.None);
            _graphicsDevice.SetRenderTarget(null);

            // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);



            VertexPositionTexture[] _vertices2 = new VertexPositionTexture[4];
            int[] _indices = new int[6];

            _vertices2[0].Position = new Vector3(-1, 1, 1);
            _vertices2[1].Position = new Vector3(1, 1, 1);
            _vertices2[2].Position = new Vector3(1, -1, 1);
            _vertices2[3].Position = new Vector3(-1, -1, 1);

            _vertices2[0].TextureCoordinate = new Vector2(0, 0);
            _vertices2[1].TextureCoordinate = new Vector2(1, 0);
            _vertices2[2].TextureCoordinate = new Vector2(0, 1);
            _vertices2[3].TextureCoordinate = new Vector2(1, 1);

            _indices[0] = 0;
            _indices[1] = 1;
            _indices[2] = 3;
            _indices[3] = 3;
            _indices[4] = 1;
            _indices[5] = 2;

            _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.ThirtyTwoBits, _indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indices);

            _graphicsDevice.Indices = _indexBuffer;
            _graphicsDevice.BlendState = BlendState.Opaque;

            _effect.CurrentTechnique = _effect.Techniques["Physik"];

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                                                             _vertices2,
                                                             0,
                                                             _vertices2.Length,
                                                             _indices,
                                                             0,
                                                             _indices.Length / 3,
                                                             VertexPositionTexture.VertexDeclaration);
            }

            _graphicsDevice.SetRenderTarget(null);

            Vector4[] data = new Vector4[SharedList.SquareSize * SharedList.SquareSize];
            rt.GetData<Vector4>(data);



            //for (int i = 0; i < SharedList.SquareSize; i++)
            //    for (int j = 0; j < SharedList.SquareSize; j++)
            //    {
            //        int index = i * SharedList.SquareSize + j;
            //        data[index].Y = (float)(50.00f * Math.Sin(((j + offset) % SharedList.SquareSize) * KreisPos));
            //        data[index].X = (float)(50.00f * Math.Cos(((j + offset) % SharedList.SquareSize) * KreisPos));
            //    }

            //KreisPos = 0.2f;
            //offset += 1;
            //rt.SetData(data);


         //   Console.WriteLine(data[0]);

            SpriteBatch _spriteBatch = new SpriteBatch(_graphicsDevice);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.Default,
                RasterizerState.CullNone);
            //  _spriteBatch.Draw(_renderTargetMain, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(rt, new Vector2(0, 0), Color.Red);
            _spriteBatch.End();

            _sharedList.ParticlePositions = rt;
        }
    }
}
