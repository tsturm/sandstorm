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
        private RenderTarget2D renderTarget1 = null;

        private IndexBuffer _indexBuffer = null;

        private static VertexDeclaration _instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
        );  

        public PhysicEngine(GraphicsDevice pGraphicsDevice, ContentManager pContentManager,SharedList pList, HeightMap heightMap)
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


            renderTarget1 = new RenderTarget2D(_graphicsDevice,512, 512, false, SurfaceFormat.Vector4, DepthFormat.None);
            
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
            Texture2D pos = _sharedList.ParticlePositions;
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
            pos.SetData(data);

            /*
            _graphicsDevice.SetRenderTarget(renderTarget1);

            _graphicsDevice.Clear(Color.Transparent);


            _graphicsDevice.SetVertexBuffers(
                       new VertexBufferBinding(_vertexBuffer, 0, 0)
           );



            
            List<short> indices = new List<short>();
            for(int i=0; i < 100;i++)
                indices.Add((short)(0));

            _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, indices.Count, BufferUsage.WriteOnly);

            _indexBuffer.SetData(indices.ToArray());

            _graphicsDevice.Indices = _indexBuffer;
            _effect.CurrentTechnique = _effect.Techniques["Physik"];

            _effect.Parameters["positionMap"].SetValue(_sharedList.ParticlePositions);

            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            _graphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //_graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertexBuffer.VertexCount, 0, _vertexBuffer.VertexCount / 3);
            }


            _sharedList.ParticlePositions = renderTarget1;

            _graphicsDevice.SetRenderTarget(null);*/
        }
    }
}
