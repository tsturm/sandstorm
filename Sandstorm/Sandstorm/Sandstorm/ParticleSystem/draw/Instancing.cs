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

            Vector2[] iVertex = new Vector2[SharedList.SquareSize * SharedList.SquareSize]; //Position auf 

            for (int x = 0; x < SharedList.SquareSize; x++)
                for (int y = 0; y < SharedList.SquareSize; y++)
                {
                    iVertex[x * SharedList.SquareSize + y].X = (float)(x) / SharedList.SquareSize;
                    iVertex[x * SharedList.SquareSize + y].Y = (float)(y) / SharedList.SquareSize;
                }


            InitInstanceVertexBuffer(iVertex);


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

        RenderTarget2D renderTarget1 = null;
        RenderTarget2D renderTarget2 = null;
        int k = 0;
        RenderTarget2D curTarget = null;
        public RenderTarget2D Draw(Camera pCamera)
        {
            if (_internalstate != _state)
            {
                _internalstate = _state;
                LoadParticleInstance();
            }

            if (renderTarget1 == null)
                renderTarget1 = new RenderTarget2D(_graphicsDevice, _graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight, false, _graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);        
            if (renderTarget2 == null)
                renderTarget2 = new RenderTarget2D(_graphicsDevice, _graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight, false, _graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            curTarget = ((k++)%2==0) ? renderTarget1 : renderTarget2;
            _graphicsDevice.SetRenderTarget(curTarget);

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
            _effect.Parameters["positionMap"].SetValue(_sharedList.ParticlePositions);
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
                                                               _indexBuffer.IndexCount / 3, SharedList.SquareSize * SharedList.SquareSize);
            }

            return curTarget;
        }
    }
}
