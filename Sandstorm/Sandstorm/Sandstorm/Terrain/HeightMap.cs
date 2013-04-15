using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Sandstorm.Terrain
{
    class HeightMap
    {
        Effect _effect;
        Texture2D _heightMap;
        GraphicsDevice _graphicsDevice;
        ContentManager _contentManager;
        VertexPositionTexture[] _vertices;
        int[] _indices;

        public HeightMap(GraphicsDevice pGraphicsDevice, ContentManager pContentManager)
        {
            _graphicsDevice = pGraphicsDevice;
            _contentManager = pContentManager;

            _effect = _contentManager.Load<Effect>("fx/terrain");
            Texture2D heightMap = _contentManager.Load<Texture2D>("tex/heightmap");

            Color[] heightMapData = new Color[heightMap.Width * heightMap.Height];
            Vector4[] heightMapData2 = new Vector4[heightMap.Width * heightMap.Height];
            heightMap.GetData(heightMapData);

            for (int i = 0; i < heightMapData.Length; i++)
            {
                heightMapData2[i].X = heightMapData[i].R / 255f;
                heightMapData2[i].Y = heightMapData[i].G / 255f;
                heightMapData2[i].Z = heightMapData[i].B / 255f;
                heightMapData2[i].W = heightMapData[i].A / 255f;
            }

            _heightMap = new Texture2D(_graphicsDevice, heightMap.Width, heightMap.Height, false, SurfaceFormat.Vector4);
            _heightMap.SetData(heightMapData2);
        }

        public void GenerateHeightField(int pWidth, int pHeight)
        {
            int heightOver2 = pHeight / 2;
            int widthOver2 = pWidth / 2;

            _vertices = new VertexPositionTexture[pWidth * pHeight];

            for (int z = 0; z < pHeight; z++)
            {
                for (int x = 0; x < pWidth; x++)
                {
                    _vertices[x + z * pWidth].Position = new Vector3(-widthOver2 + x, 0f, -heightOver2 + z);
                    _vertices[x + z * pWidth].TextureCoordinate = new Vector2((float)x/(float)pWidth, (float)z/(float)pHeight);
                }
            }

            _indices = new int[(pWidth - 1) * (pHeight - 1) * 6];
            int counter = 0;
            for (int z = 0; z < pHeight - 1; z++)
            {
                for (int x = 0; x < pWidth - 1; x++)
                {
                    _indices[counter++] = x + z * pWidth;
                    _indices[counter++] = (x+1) + z * pWidth; 
                    _indices[counter++] = (x+1) + (z+1) * pWidth;

                    _indices[counter++] = (x+1) + (z+1) * pWidth;
                    _indices[counter++] = x + (z+1) * pWidth;
                    _indices[counter++] = x + z * pWidth;
                }
            }
        }

        public void Update(Texture2D pHeightMap)
        {

        }

        public void Draw(Camera pCamera)
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            //rs.FillMode = FillMode.WireFrame;
            _graphicsDevice.RasterizerState = rs;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;

            _effect.CurrentTechnique = _effect.Techniques["Terrain"];
            _effect.Parameters["viewMatrix"].SetValue(pCamera.ViewMatrix);
            _effect.Parameters["projMatrix"].SetValue(pCamera.ProjMatrix);
            _effect.Parameters["worldMatrix"].SetValue(Matrix.Identity);
            _effect.Parameters["heightMap"].SetValue(_heightMap);

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
                                                         VertexPositionTexture.VertexDeclaration);
            }
        }
    }
}
