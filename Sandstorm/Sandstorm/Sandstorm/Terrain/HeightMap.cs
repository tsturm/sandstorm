using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Sandstorm.Terrain
{
    public class HeightMap
    {
        private static short HMSIZE = 512;

        Effect _effect;
        Texture2D _heightMap;
        GraphicsDevice _graphicsDevice;
        ContentManager _contentManager;
        VertexPositionTexture[] _vertices;
        Matrix _transform;
        int[] _indices;
        private Object updateLock = new Object();
        public float _heightScale = 100.0f;
        public float _contourSpacing = 4.5f;
        public bool _displayContours = true;

        public Vector4 _color0 = new Vector4(0.0f, 0.0f, 0.65f, 1.0f);
        public Vector4 _color1 = new Vector4(0.2f, 0.52f, 0.03f, 1.0f);
        public Vector4 _color2 = new Vector4(0.9f, 0.85f, 0.34f, 1.0f);
        public Vector4 _color3 = new Vector4(0.7f, 0.17f, 0.0f, 1.0f);

        float[,] _heightData = new float[HMSIZE, HMSIZE];
        Vector3[,] _normals = new Vector3[HMSIZE, HMSIZE];


        RenderTarget2D _rt = null;
        
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
                heightMapData2[i].X = heightMapData[i].R / 255f;
                heightMapData2[i].Y = heightMapData[i].G / 255f;
                heightMapData2[i].Z = heightMapData[i].B / 255f;
                heightMapData2[i].W = 1.0f;
            }

            _heightMap = new Texture2D(_graphicsDevice, heightMap.Width, heightMap.Height, false, SurfaceFormat.Vector4);

            _heightMap.SetData(heightMapData2);

            _rt = new RenderTarget2D(_graphicsDevice, _graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        }

        private void setFinishedData(Texture2D pTex, float[,] pHeightData, Vector3[,] pNormals)
        {
            lock (this.updateLock)
            {
                _graphicsDevice.Textures[0] = null;
                _graphicsDevice.Textures[1] = null;
                _graphicsDevice.Textures[2] = null;
                _graphicsDevice.Textures[3] = null;
                _heightMap.Dispose();

                _heightMap = pTex;

                _heightData = pHeightData;

                _normals = pNormals;
            }
        }

        Thread myThread = null;
        public void setData(short[] data, int width, int height)
        {
           // width = height = 420;
            if (myThread == null)
            {
                myThread = new Thread(delegate()
                {
                    Debug.WriteLine("Start:" + System.Environment.TickCount);

                    Texture2D heightMap = new Texture2D(_graphicsDevice, 420, 420, false, SurfaceFormat.Vector4);

                    Vector4[] myVector = new Vector4[420 * 420];
                    float[,] heightData = new float[420, 420];
                    Vector3[,] normals = new Vector3[420, 420];

                    {
                        Debug.WriteLine("StartHeight:" + System.Environment.TickCount);
                        int tWidth = width;
                        int tHeight = height;
                        //parse depthimage to vector

                        short myActualPixel;
                        for (int idx = 0, y = 30; y < tHeight - 30; y++)
                        {
                            for (int x = 110; x < tWidth - 110; x++, idx++)
                            {
                                myActualPixel = (short)(data[y * tWidth + x] - 1000);
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
                                heightData[x - 110, y - 30] = myVector[idx].Y * _heightScale;
                            }
                        }
                        Debug.WriteLine("EndHeight:" + System.Environment.TickCount);
                    }
                    {
                        int tWidth = 420;
                        int tHeight = 420;
                        for (int x = 0; x < tWidth - 1; x++)
                        {
                            for (int y = 0; y < tHeight - 1; y++)
                            {
                                Vector3 v = new Vector3(x, heightData[x, y], y);
                                Vector3 v1 = new Vector3(x + 1, heightData[x + 1, y], y);
                                Vector3 v2 = new Vector3(x, heightData[x, y + 1], y + 1);
                                Vector3 normal = Vector3.Cross(v - v1, v - v2);
                                normal.Normalize();
                                normals[x, y] = -1 * normal;
                            }
                        }
                    }
                    heightMap.SetData(myVector);

                    Debug.WriteLine("End:" + System.Environment.TickCount);
                    setFinishedData(heightMap, heightData, normals);
                    Debug.WriteLine("Finished:" + System.Environment.TickCount);
                    myThread = null;
                });

                myThread.Name = "Heigmap-Updater";
                myThread.Start();
            }

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

        public Texture2D Draw(Camera pCamera)
        {
            _graphicsDevice.SetRenderTarget(_rt);
           // lock (this.updateLock)
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
                _effect.Parameters["heightScale"].SetValue(_heightScale);
                _effect.Parameters["color0"].SetValue(_color0);
                _effect.Parameters["color1"].SetValue(_color1);
                _effect.Parameters["color2"].SetValue(_color2);
                _effect.Parameters["color3"].SetValue(_color3);
                _effect.Parameters["contourSpacing"].SetValue(_contourSpacing);
                _effect.Parameters["displayContours"].SetValue(_displayContours);

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
            _graphicsDevice.SetRenderTarget(null);
            return _rt;
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
            return 0;
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

        /*private float clacHeight(Texture2D pTex,int x, int y)
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
                pTex.GetData<Vector4>(
                    0,
                    sourceRectangle,
                    retrievedColor,
                    0,
                    retrievedColor.Length);
                return ((retrievedColor[0].X) * 100);
            }
            return 0;
        }*/

        public int getWidth()
        {
            return _heightMap.Width;
        }
    }
}
