using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using SandstormKinect;

namespace Sandstorm.Terrain
{
    public class HeightMap
    {
        Effect _effect;
        Texture2D _heightMap;
        GraphicsDevice _graphicsDevice;
        ContentManager _contentManager;
        VertexPositionTexture[] _vertices;
        Matrix _transform;
        int[] _indices;
        

        float[,] _heightData;
        Vector3[,] _normals;

        public HeightMap(GraphicsDevice pGraphicsDevice, ContentManager pContentManager)
        {
            _graphicsDevice = pGraphicsDevice;
            _contentManager = pContentManager;

            _transform = new Matrix();

            _transform = Matrix.CreateScale(1, 1, -1);

            _effect = _contentManager.Load<Effect>("fx/terrain");
            Texture2D heightMap = _contentManager.Load<Texture2D>("tex/heightmap");

            Color[] heightMapData = new Color[heightMap.Width * heightMap.Height];
            Vector4[] heightMapData2 = new Vector4[heightMap.Width * heightMap.Height];
            heightMap.GetData(heightMapData);

            for (int i = 0; i < heightMapData.Length; i++)
            {
                heightMapData2[i].X = ((heightMapData[i].R / 255f) + 1f) / 2f;
                heightMapData2[i].Y = ((heightMapData[i].G / 255f) + 1f) / 2f;
                heightMapData2[i].Z = ((heightMapData[i].B / 255f) + 1f) / 2f;
                heightMapData2[i].W = ((heightMapData[i].A / 255f) + 1f) / 2f;
            }

            _heightMap = new Texture2D(_graphicsDevice, heightMap.Width, heightMap.Height, false, SurfaceFormat.Vector4);

            _heightMap.SetData(heightMapData2);

            initHeightData();
        }

        private void initHeightData()
        {
            _heightData = new float[_heightMap.Width, _heightMap.Height];
            for (int x = 0; x < _heightMap.Width; x++)
                for (int y = 0; y < _heightMap.Height; y++)
                    _heightData[x, y] = clacHeight(x - (_heightMap.Width / 2), y - (_heightMap.Height / 2));
            _normals = new Vector3[_heightMap.Width, _heightMap.Height];
            for (int x = 0; x < _heightMap.Width - 1; x++)
                for (int y = 0; y < _heightMap.Height - 1; y++)
                {
                    Vector3 v = new Vector3(x, _heightData[x, y], y);
                    Vector3 v1 = new Vector3(x + 1, _heightData[x + 1, y], y);
                    Vector3 v2 = new Vector3(x, _heightData[x, y + 1], y + 1);
                    Vector3 normal = Vector3.Cross(v - v1, v - v2);
                    normal.Normalize();
                    _normals[x, y] = -1 * normal;
                }
        }

        public void setData(short[] data, int width, int height)
        {
            _graphicsDevice.Textures[0] = null;
            _graphicsDevice.Textures[1] = null;
            _graphicsDevice.Textures[2] = null;
            _graphicsDevice.Textures[3] = null;
            _heightMap.Dispose();
            _heightMap = new Texture2D(_graphicsDevice, 420, 420, false, SurfaceFormat.Vector4);

            //parse depthimage to vector
            Vector4[] myVector = new Vector4[420 * 420];
            
            for (int idx=0, y = 30; y < height - 30; y++)
            {
                for (int x = 110; x < width - 110; x++, idx++)
                {
                    short myActualPixel = (short)(data[y * width + x] - 1000);
                    if (myActualPixel > 0 && myActualPixel <= 230)
                    {
                        //validen Bildpunkt gefunden
                        myVector[idx].X = (float)-(((float)myActualPixel / 230f) - 1);
                        myVector[idx].Y = (float)-(((float)myActualPixel / 230f) - 1);
                        myVector[idx].Z = (float)-(((float)myActualPixel / 230f) - 1);
                        myVector[idx].W = 1f;
                    }
                    else
                    {
                        myVector[idx].X = 0f;
                        myVector[idx].Y = 0f;
                        myVector[idx].Z = 0f;
                        myVector[idx].W = 1f;
                    }
                }
            }
            // set data
            _heightMap.SetData(myVector);

            initHeightData();
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
                    _vertices[x + z * pWidth].TextureCoordinate = new Vector2((float)x / (float)pWidth, (float)z / (float)pHeight);
                }
            }

            _indices = new int[(pWidth - 1) * (pHeight - 1) * 6];
            int counter = 0;
            for (int z = 0; z < pHeight - 1; z++)
            {
                for (int x = 0; x < pWidth - 1; x++)
                {
                    _indices[counter++] = x + z * pWidth;
                    _indices[counter++] = (x + 1) + z * pWidth;
                    _indices[counter++] = (x + 1) + (z + 1) * pWidth;

                    _indices[counter++] = (x + 1) + (z + 1) * pWidth;
                    _indices[counter++] = x + (z + 1) * pWidth;
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
            _effect.Parameters["worldMatrix"].SetValue(_transform);
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
        public float getHeight(int x, int y)
        {
            int xpos = (int)x + (_heightMap.Width / 2);
            int ypos = (int)y + (_heightMap.Height / 2);
            if (
                xpos < _heightMap.Width &&
                ypos < _heightMap.Height &&
                xpos >= 0 &&
                ypos >= 0)
            {
                return _heightData[(int)(xpos), (int)(ypos)];
            }
            return 100000;
        }

        public float getHeightData(float x, float y)
        {

            float relx = x % 1;
            float rely = y % 1;
            float h = getHeight((int)(x - relx), (int)(y - rely));
            float h1 = h - getHeight((int)(x - relx + 1), (int)(y - rely));
            float h2 = h - getHeight((int)(x - relx), (int)(y - rely + 1));
            float ret = h + ((Math.Abs(h1 * relx) + Math.Abs(h2 * rely)) / 2);
            return ret;
        }

        public Vector3 getNormal(float x, float y)
        {
            return getNormal((int)x, (int)y);
        }

        public Vector3 getNormal(int x, int y)
        {
            int xpos = (int)x + (_heightMap.Width / 2);
            int ypos = (int)y + (_heightMap.Height / 2);
            if (
                xpos < _heightMap.Width &&
                ypos < _heightMap.Height &&
                xpos >= 0 &&
                ypos >= 0)
            {
                return _normals[xpos, ypos];
            }
            return new Vector3(0, 1, 0);
        }

        private float clacHeight(int x, int y)
        {
            int xpos = (int)x + (_heightMap.Width / 2);
            int ypos = (int)y + (_heightMap.Height / 2);
            if (
                xpos < _heightMap.Width &&
                ypos < _heightMap.Height &&
                xpos >= 0 &&
                ypos >= 0)
            {
                Rectangle sourceRectangle = new Rectangle(xpos, ypos, 1, 1);
                //Color[] retrievedColor = new Color[4];
                Vector4[] retrievedColor = new Vector4[1];
                _heightMap.GetData<Vector4>(
                    0,
                    sourceRectangle,
                    retrievedColor,
                    0,
                    retrievedColor.Length);
                return ((retrievedColor[0].X) * 100);
            }
            return 100000;
        }

        public int getWidth()
        {
            return _heightMap.Width;
        }
    }
}
