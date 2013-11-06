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
        Vector2[] _vertices = null;
        int[] _indices = null;

        private static VertexDeclaration _VertexDeclaration = new VertexDeclaration
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




            _vertices = new Vector2[4];
            _vertexBuffer = new VertexBuffer(_graphicsDevice, _VertexDeclaration, _vertices.Length, BufferUsage.None);
            _vertices[0] = new Vector2(-1, 1);  //oben links
            _vertices[1] = new Vector2(1, 1);   //oben rechts
            _vertices[2] = new Vector2(1, -1);  //unten rechts
            _vertices[3] = new Vector2(-1, -1); //unten links


            _indices = new int[6];
            _indices[0] = 0;
            _indices[1] = 1;
            _indices[2] = 3;
            _indices[3] = 2;
            _indices[4] = 3;
            _indices[5] = 1;
            _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.ThirtyTwoBits, _indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indices);


            _effect = _contentManager.Load<Effect>("fx/Physik");
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
        private Texture2D doPhysicsCPU()
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
        }

        bool up = false;
        private Texture2D doPhysicsGPU()
        {
            RenderTarget2D rt = new RenderTarget2D(_graphicsDevice, SharedList.SquareSize, SharedList.SquareSize, false, SurfaceFormat.Vector4, DepthFormat.None);
            _graphicsDevice.SetRenderTarget(rt);
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;
            _graphicsDevice.BlendState = BlendState.Opaque;

            _effect.CurrentTechnique = _effect.Techniques["Physik"];
            _effect.Parameters["wavePos"].SetValue(KreisPos);

            if(up)
                KreisPos += 0.01f;               
            else
                KreisPos -= 0.01f;
            if (KreisPos > 1.0f)
                up = false;
            else if (KreisPos <= 0.2f)
                up = true;

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
            return rt;
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
        public void Draw() //Nothing to draw.. normally
        {
            Texture2D pos = doPhysicsGPU();
            ShowTextureTopLeft(pos);
             _sharedList.ParticlePositions = pos;
        }
    }
}
