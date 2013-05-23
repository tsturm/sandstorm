using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sandstorm.ParticleSystem.structs;

using System.Threading;
using System.Threading.Tasks;

namespace Sandstorm.ParticleSystem.draw
{
    class Instancing
    {
        private Camera _camera = null;
        private GraphicsDevice _graphicsDevice = null;
        private ContentManager _contentManager = null;
        private SharedList _sharedList = null;


        private DynamicVertexBuffer instanceVertexBuffer = null;
        private Vector3[] _instanceTransforms;        
        private Effect _effect;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;           
        private Texture2D _billboardTexture;

        private static VertexDeclaration _instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 1)
        );        

        private void LoadParticleInstance()
        {
            List<BillboardVertex> vertices = new List<BillboardVertex>();
            List<short> indices = new List<short>();
            short baseIndex = (short)0;

            BillboardVertex vertex = new BillboardVertex();

            vertex.Position = new Vector4(1f, 1f, 1f, 1f);
            vertex.TexCoord = new Vector4(0.0f, 0.0f, -1.0f, 1.0f);
            vertices.Add(vertex);

            vertex.Position = new Vector4(1, 1, 1, 1);
            vertex.TexCoord = new Vector4(1.0f, 0.0f, 1.0f, 1.0f);
            vertices.Add(vertex);

            vertex.Position = new Vector4(1, 1, 1, 0);
            vertex.TexCoord = new Vector4(0.0f, 1.0f, -1.0f, -1.0f);
            vertices.Add(vertex);

            vertex.Position = new Vector4(1, 1, 1, 0);
            vertex.TexCoord = new Vector4(1.0f, 1.0f, 1.0f, -1.0f);
            vertices.Add(vertex);

            indices.Add((short)(0 + baseIndex));
            indices.Add((short)(1 + baseIndex));
            indices.Add((short)(2 + baseIndex));
            indices.Add((short)(2 + baseIndex));
            indices.Add((short)(1 + baseIndex));
            indices.Add((short)(3 + baseIndex));

            _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(BillboardVertex), vertices.Count, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices.ToArray());

            _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, indices.Count, BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices.ToArray());
        }

        public Instancing(GraphicsDevice pGraphicsDevice, ContentManager pContentManager, Camera pCamera, SharedList pList)
        {
            // TODO: Complete member initialization

            this._camera = pCamera;
            this._graphicsDevice = pGraphicsDevice;
            this._contentManager = pContentManager;
            this._sharedList = pList;
            _effect = _contentManager.Load<Effect>("fx/InstancedModel");
            _billboardTexture = _contentManager.Load<Texture2D>("tex/particle"); 

            LoadParticleInstance();
        }


        void DrawInstances(VertexBuffer vertexBuffer, IndexBuffer indexBuffer, Texture2D pTexture, Vector3[] pInstances)
        {
            if (pInstances.Length == 0)
                return;

            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((instanceVertexBuffer == null) ||
                (pInstances.Length > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                    instanceVertexBuffer.Dispose();

                instanceVertexBuffer = new DynamicVertexBuffer(_graphicsDevice, _instanceVertexDeclaration,
                                                               pInstances.Length, BufferUsage.WriteOnly);
            }

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            instanceVertexBuffer.SetData(pInstances, 0, pInstances.Length, SetDataOptions.Discard);
                      
            // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
             _graphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(vertexBuffer,0,0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
            );

            _graphicsDevice.Indices = indexBuffer;

            // Set up the instance rendering effect.
            _effect.CurrentTechnique = _effect.Techniques["InstancingBB"];

            _effect.Parameters["world"].SetValue(Matrix.Identity);
            _effect.Parameters["view"].SetValue(_camera.ViewMatrix);
            _effect.Parameters["projection"].SetValue(_camera.ProjMatrix);
            _effect.Parameters["Texture"].SetValue(_billboardTexture);
            _effect.Parameters["alphaTestDirection"].SetValue(1.0f);
            _effect.Parameters["alphaTestThreshold"].SetValue(0.8f);
            

            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphicsDevice.RasterizerState = RasterizerState.CullNone;


            // Draw all the instance copies in a single call.
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               vertexBuffer.VertexCount, 0,
                                                               indexBuffer.IndexCount / 3, pInstances.Length);
            }
        }


        public void Draw()
        {
            Array.Resize(ref _instanceTransforms, _sharedList.Count);

            //TODO: Parallelisieren?!
            //Parallel.For(0, _sharedList.Particles.Length, i =>
            int k=0;
            for(int i=0; i < _sharedList.Particles.Length;i++)
            {
                Particle p = _sharedList.Particles[i];
                if(p != null)
                    _instanceTransforms[k++] = p.Pos;
            }//);

            DrawInstances(_vertexBuffer, _indexBuffer, _billboardTexture, _instanceTransforms);
        }
    }
}
