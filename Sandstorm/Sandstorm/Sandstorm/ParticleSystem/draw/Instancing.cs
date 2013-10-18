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

        Texture2D _particlePositions;
        public enum INSTANCE_MODE
        {
            NORMAL,
            DEBUG
        }

        private GraphicsDevice _graphicsDevice = null;
        private ContentManager _contentManager = null;
        private SharedList _sharedList = null;
        
        private DynamicVertexBuffer instanceVertexBuffer = null;
        private Effect _effect;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;           
        private Texture2D _billboardTexture;

        private static float _BBSize = 25.0f;
        
        private static float _DebugBBSize = 1.0f;
        private static INSTANCE_MODE _state = INSTANCE_MODE.NORMAL;
        private static INSTANCE_MODE _internalstate = INSTANCE_MODE.NORMAL;

        private static int _squareSize = 16;

        private static VertexDeclaration _instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 1)
        );  
      
        public static void nextMode()
        {
            switch (_state)
            {
                case INSTANCE_MODE.NORMAL:
                    _state = INSTANCE_MODE.DEBUG;
                    break;
                case INSTANCE_MODE.DEBUG:
                    _state = INSTANCE_MODE.NORMAL;
                    break;
            }
        }

        private void LoadParticleInstance()
        {            
            float pSize=0;
            switch (_state)
            {
                case INSTANCE_MODE.DEBUG:
                    pSize = _DebugBBSize;
                    break;
                default:
                    pSize= _BBSize;
                    break;
            }

            List<BillboardVertex> vertices = new List<BillboardVertex>();
            List<short> indices = new List<short>();
            short baseIndex = (short)0;

            BillboardVertex vertex = new BillboardVertex();

            vertex.Position = new Vector4(1f, 1f, 1f, 1f);
            vertex.TexCoord = new Vector4(0.0f, 0.0f, -pSize, pSize);
            vertices.Add(vertex);

            vertex.Position = new Vector4(1, 1, 1, 1);
            vertex.TexCoord = new Vector4(1.0f, 0.0f, pSize, pSize);
            vertices.Add(vertex);

            vertex.Position = new Vector4(1, 1, 1, 0);
            vertex.TexCoord = new Vector4(0.0f, 1.0f, -pSize, -pSize);
            vertices.Add(vertex);

            vertex.Position = new Vector4(1, 1, 1, 0);
            vertex.TexCoord = new Vector4(1.0f, 1.0f, pSize, -pSize);
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

        public Instancing(GraphicsDevice pGraphicsDevice, ContentManager pContentManager, SharedList pList)
        {
            this._graphicsDevice = pGraphicsDevice;
            this._contentManager = pContentManager;
            this._sharedList = pList;
            _effect = _contentManager.Load<Effect>("fx/particleDrawer");
            _billboardTexture = _contentManager.Load<Texture2D>("tex/smoke"); 

            LoadParticleInstance();
            _particlePositions = new Texture2D(_graphicsDevice, _squareSize, _squareSize, false, SurfaceFormat.Vector4);


            Vector2[] iVertex = new Vector2[_squareSize * _squareSize]; //Position auf 

            for (int x = 0; x < _squareSize; x++)
                for (int y = 0; y < _squareSize; y++)
                {
                    iVertex[x * _squareSize + y].X = (float)(x) / _squareSize;
                    iVertex[x * _squareSize + y].Y = (float)(y) / _squareSize;
                }

            /*for (int i = 0; i < _squareSize * _squareSize; i++)
            {
                iVertex[i] = 1.0f*i;
            }*/

            InitInstanceVertexBuffer(iVertex);


            Vector4[] myVector = new Vector4[_squareSize * _squareSize];

            Random r = new Random();
            for (int x = 0; x < _squareSize; x++)
                for (int y = 0; y < _squareSize; y++)
                {
                    myVector[x * _squareSize + y].X = x * 10f;
                    myVector[x * _squareSize + y].Y = y * 10f;
                }
            _particlePositions.SetData(myVector);

        }

        void InitInstanceVertexBuffer(Vector2[] pPositions)
        {
            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((instanceVertexBuffer == null) || (pPositions.Length > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                    instanceVertexBuffer.Dispose();

                instanceVertexBuffer = new DynamicVertexBuffer(_graphicsDevice, _instanceVertexDeclaration,
                                                               pPositions.Length, BufferUsage.None);
            }

            instanceVertexBuffer.SetData(pPositions, 0, pPositions.Length, SetDataOptions.Discard);
        }

        RenderTarget2D lastRenderTarget = null;
        RenderTarget2D renderTarget = null;
        public RenderTarget2D Draw(Camera pCamera, RenderTarget2D pTarget)
        {
            if (_internalstate != _state)
            {
                _internalstate = _state;
                LoadParticleInstance();
            }
            
            renderTarget = new RenderTarget2D(_graphicsDevice, _graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _graphicsDevice.SetRenderTarget(renderTarget);

            _graphicsDevice.Clear(Color.Transparent);

            // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
            _graphicsDevice.SetVertexBuffers(
                       new VertexBufferBinding(_vertexBuffer, 0, 0),
                       new VertexBufferBinding(instanceVertexBuffer, 0, 1)
           );

            _graphicsDevice.Indices = _indexBuffer;

            // Set up the instance rendering effect.
            _effect.CurrentTechnique = _effect.Techniques["InstancingBB"];


            _effect.Parameters["world"].SetValue(Matrix.Identity);
            _effect.Parameters["view"].SetValue(pCamera.ViewMatrix);
            _effect.Parameters["projection"].SetValue(pCamera.ProjMatrix);
            _effect.Parameters["Texture"].SetValue(_billboardTexture);
            _effect.Parameters["positionMap"].SetValue(_particlePositions);
            /*_effect.Parameters["alphaTestDirection"].SetValue(1.0f);
            _effect.Parameters["alphaTestThreshold"].SetValue(0.3f);*/
            _effect.Parameters["BBSize"].SetValue((_state == INSTANCE_MODE.DEBUG) ? _DebugBBSize : _BBSize);
            _effect.Parameters["debug"].SetValue((_state == INSTANCE_MODE.DEBUG) ? true : false);


            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            _graphicsDevice.RasterizerState = RasterizerState.CullNone;


            // Draw all the instance copies in a single call.
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               _vertexBuffer.VertexCount, 0,
                                                               _indexBuffer.IndexCount / 3, _squareSize * _squareSize);
            }

            lastRenderTarget = renderTarget;
            return lastRenderTarget;
        }
    }
}
