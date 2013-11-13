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
        VertexPositionTexture[] _vertices = null;


        RenderTarget2D _rt = null;
        int[] _indices = null;


        private Texture2D _particlePositions = null;

        private VertexDeclaration _VertexDeclaration = null;

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

            _VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

            _rt = new RenderTarget2D(_graphicsDevice, SharedList.SquareSize, SharedList.SquareSize, false, SurfaceFormat.Vector4, DepthFormat.None);

            _vertices = new VertexPositionTexture[4];
            _vertexBuffer = new VertexBuffer(_graphicsDevice, _VertexDeclaration, _vertices.Length, BufferUsage.None);
            _vertices[0].Position = new Vector3(-1, 1, 1);  //oben links
            _vertices[1].Position = new Vector3(1, 1, 1);   //oben rechts
            _vertices[2].Position = new Vector3(1, -1, 1);  //unten rechts
            _vertices[3].Position = new Vector3(-1, -1, 1); //unten links

            _vertices[0].TextureCoordinate = new Vector2(0, 0);  //oben links
            _vertices[1].TextureCoordinate = new Vector2(1, 0);   //oben rechts
            _vertices[2].TextureCoordinate = new Vector2(1, 1);  //unten rechts
            _vertices[3].TextureCoordinate = new Vector2(0, 1); //unten links


            _indices = new int[6];
            _indices[0] = 0;
            _indices[1] = 1;
            _indices[2] = 3;
            _indices[3] = 3;
            _indices[4] = 1;
            _indices[5] = 2;
            _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.ThirtyTwoBits, _indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indices);


            _effect = _contentManager.Load<Effect>("fx/Physik");

            _particlePositions = new Texture2D(_graphicsDevice, SharedList.SquareSize, SharedList.SquareSize, false, SurfaceFormat.Vector4);//new Texture2D(_graphicsDevice, SharedList.SquareSize, SharedList.SquareSize, false, SurfaceFormat.Vector4);

            Vector4[] myVector = new Vector4[SharedList.SquareSize * SharedList.SquareSize];
            for (int x = 0; x < SharedList.SquareSize; x++)
                for (int y = 0; y < SharedList.SquareSize; y++)
                {
                    myVector[x * SharedList.SquareSize + y].X = x;
                    myVector[x * SharedList.SquareSize + y].Y = y;
                    myVector[x * SharedList.SquareSize + y].Z = 1.0f;
                    myVector[x * SharedList.SquareSize + y].W = 1.0f;
                }
            _particlePositions.SetData(myVector);
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

        float KreisPos = 0;
        float offset = 0;
      /*  private Texture2D doPhysicsCPU()
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
                    data[index].Y = (float)(50.00f * Math.Sin(((j + offset) % SharedList.SquareSize) * KreisPos));
                }

            KreisPos = 0.2f;
            offset += 0.1f;
            pos.SetData(data);
            return pos;
        }*/

        private Texture2D doPhysicsGPU()
        {
            _graphicsDevice.BlendState = BlendState.Opaque;

            _graphicsDevice.SetRenderTarget(_rt); //ins Rendertarget rendern

            _graphicsDevice.Clear(Color.White); //Rendertarget löschen
            _graphicsDevice.SetVertexBuffer(_vertexBuffer); //Vertexbuffer setzen
            _graphicsDevice.Indices = _indexBuffer; //Indexbuffer setzen
            

            _effect.CurrentTechnique = _effect.Techniques["Move"];
            _effect.Parameters["positionMap"].SetValue(_particlePositions);
            
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                                                             _vertices,
                                                             0,
                                                             _vertices.Length,
                                                             _indices,
                                                             0,
                                                             _indices.Length / 3,
                                                             _VertexDeclaration);
            }
            
            _graphicsDevice.SetRenderTarget(null);

            return _rt;
        }

        private void ShowTextureTopLeft(Texture2D pos)
        {
            SpriteBatch _spriteBatch = new SpriteBatch(_graphicsDevice);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.Default,
                RasterizerState.CullNone);
            //  _spriteBatch.Draw(_renderTargetMain, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(pos, new Vector2(0, 0), Color.White);
            _spriteBatch.End();
        }
        public Texture2D Draw() //Nothing to draw.. normally
        {
            Texture2D positions = doPhysicsGPU(); //Particlepositionen berechnen
           // ShowTextureTopLeft(_rt);
            return positions; //Particlepositionen zurückgeben
            
        }
    }
}
