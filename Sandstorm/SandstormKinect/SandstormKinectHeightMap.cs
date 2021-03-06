﻿using System;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Content;
using SandstormKinect;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace SandstormKinect
{
    public class SandstormKinectHeightMap
    {
        /*

        #region FIELDS

        private Effect                      m_Effect;
        private Texture2D                   m_HeightMap;
        private GraphicsDevice              m_GraphicsDevice;
        private ContentManager              m_ContentManager;
        private VertexPositionTexture[]     m_Vertices;
        private Matrix                      m_TransformMatrix;
        private int[]                       m_Indices;
        
        private float                       m_HeightScale;
        private float                       m_ContourSpacing;
        private bool                        m_DisplayContoursFlag;

        private Object                      m_UpdateLock;     //Object for locking              // = new Object();
        
        //public float _heightScale = 100.0f;
        //public float _contourSpacing = 4.5f;
        //public bool _displayContours = true;

        public Vector4 _color0 = new Vector4(0.0f, 0.0f, 0.65f, 1.0f);
        public Vector4 _color1 = new Vector4(0.2f, 0.52f, 0.03f, 1.0f);
        public Vector4 _color2 = new Vector4(0.9f, 0.85f, 0.34f, 1.0f);
        public Vector4 _color3 = new Vector4(0.7f, 0.17f, 0.0f, 1.0f);     

        float[,] _heightData = new float[512, 512];
        Vector3[,] _normals = new Vector3[512, 512];

        RenderTarget2D _targetMain = null;
        RenderTarget2D _targetNormal = null;
        RenderTargetBinding[] _bindings = null;

        #endregion

        #region PROPPERTIES

        public float HeightScale
        {
            get { return m_HeightScale; }
            set { m_HeightScale = value; }
        }

        #endregion

        #region CONSTRUCTOR

        public SandstormKinectHeightMap(GraphicsDevice pGraphicsDevice, ContentManager pContentManager,RenderTarget2D pTargetMain,RenderTarget2D pTargetNormal)
        {
            m_GraphicsDevice = pGraphicsDevice;
            m_ContentManager = pContentManager;
            _targetNormal = pTargetNormal;
            _targetMain = pTargetMain;
            
            _bindings = new RenderTargetBinding[] {
                    new RenderTargetBinding(_targetMain),
                    new RenderTargetBinding(_targetNormal)
            };

            m_TransformMatrix = new Matrix();

            m_TransformMatrix = Matrix.CreateScale(1, 1, -1);

            m_Effect = m_ContentManager.Load<Effect>("fx/terrain");
            Texture2D heightMap = m_ContentManager.Load<Texture2D>("tex/heightmap");

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

            m_HeightMap = new Texture2D(m_GraphicsDevice, heightMap.Width, heightMap.Height, false, SurfaceFormat.Vector4);

            m_HeightMap.SetData(heightMapData2);

            //initHeightData();
        }

        #endregion

        #region PRIVATE METHODS

        private void setFinishedData(Texture2D pTex, float[,] pHeightData, Vector3[,] pNormals)
        {
            lock (this.m_UpdateLock)
            {
                m_GraphicsDevice.Textures[0] = null;
                m_GraphicsDevice.Textures[1] = null;
                m_GraphicsDevice.Textures[2] = null;
                m_GraphicsDevice.Textures[3] = null;
                m_HeightMap.Dispose();

                m_HeightMap = pTex;

                _heightData = pHeightData;

                _normals = pNormals;
            }
        }

        #endregion

        #region PUBLIC METHODS

        Thread myThread = null;
        public void setData(short[] data, int width, int height)
        {
           // width = height = 420;
            if (myThread == null)
            {
                myThread = new Thread(delegate()
                {
                    Debug.WriteLine("Start:" + System.Environment.TickCount);

                    Texture2D heightMap = new Texture2D(m_GraphicsDevice, 420, 420, false, SurfaceFormat.Vector4);

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
                                heightData[x - 110, y - 30] = myVector[idx].Y * m_HeightScale;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pWidth"></param>
        /// <param name="pHeight"></param>
        public void GenerateHeightField(int pWidth, int pHeight)
        {
            int heightOver2 = pHeight / 2;
            int widthOver2 = pWidth / 2;

            m_Vertices = new VertexPositionTexture[pWidth * pHeight];

            for (int z = 0; z < pHeight; z++)
            {
                for (int x = 0; x < pWidth; x++)
                {
                    m_Vertices[x + z * pWidth].Position = new Vector3(-widthOver2 + x, 0f, -heightOver2 + z);
                    m_Vertices[x + z * pWidth].TextureCoordinate = new Vector2((float)x / (float)pWidth, (float)z / (float)pHeight);
                }
            }

            m_Indices = new int[(pWidth - 1) * (pHeight - 1) * 6];
            int counter = 0;
            for (int z = 0; z < pHeight - 1; z++)
            {
                for (int x = 0; x < pWidth - 1; x++)
                {
                    m_Indices[counter++] = x + z * pWidth;
                    m_Indices[counter++] = (x + 1) + z * pWidth;
                    m_Indices[counter++] = (x + 1) + (z + 1) * pWidth;

                    m_Indices[counter++] = (x + 1) + (z + 1) * pWidth;
                    m_Indices[counter++] = x + (z + 1) * pWidth;
                    m_Indices[counter++] = x + z * pWidth;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pHeightMap"></param>
        public void Update(Texture2D pHeightMap)
        {

        }

        /// <summary>
        /// draw within call class
        /// </summary>
        /// <param name="pCamera"></param>
        /*
        public void Draw(Camera pCamera)
        {
           // lock (this.updateLock)
            {
                RasterizerState rs = new RasterizerState();
                rs.CullMode = CullMode.None;
                //rs.FillMode = FillMode.WireFrame;
                m_GraphicsDevice.RasterizerState = rs;
                m_GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                m_Effect.CurrentTechnique = m_Effect.Techniques["Terrain"];
                m_Effect.Parameters["viewMatrix"].SetValue(pCamera.ViewMatrix);
                m_Effect.Parameters["projMatrix"].SetValue(pCamera.ProjMatrix);
                m_Effect.Parameters["worldMatrix"].SetValue(m_TransformMatrix);
                m_Effect.Parameters["heightMap"].SetValue(m_HeightMap);
                m_Effect.Parameters["heightScale"].SetValue(m_HeightScale);
                m_Effect.Parameters["color0"].SetValue(_color0);
                m_Effect.Parameters["color1"].SetValue(_color1);
                m_Effect.Parameters["color2"].SetValue(_color2);
                m_Effect.Parameters["color3"].SetValue(_color3);
                m_Effect.Parameters["contourSpacing"].SetValue(m_ContourSpacing);
                m_Effect.Parameters["displayContours"].SetValue(m_DisplayContoursFlag);

                foreach (EffectPass pass in m_Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    m_GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                                                             m_Vertices,
                                                             0,
                                                             m_Vertices.Length,
                                                             m_Indices,
                                                             0,
                                                             m_Indices.Length / 3,
                                                             VertexPositionTexture.VertexDeclaration);
                }
            }
        }
        */
        /*
        /// <summary>
        /// get Height at Point XY
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns></returns>
        public float getHeight(int x, int y)
        {
            int xpos = (int)x + (m_HeightMap.Width / 2);
            int ypos = (int)y + (m_HeightMap.Height / 2);
            if (
                xpos < m_HeightMap.Width &&
                ypos < m_HeightMap.Height &&
                xpos >= 0 &&
                ypos >= 0)
            {
                return _heightData[(int)(xpos), (int)(ypos)];
            }
            return 0;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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
            int xpos = (int)x + (m_HeightMap.Width / 2);
            int ypos = (int)y + (m_HeightMap.Height / 2);
            if (
                xpos < m_HeightMap.Width &&
                ypos < m_HeightMap.Height &&
                xpos >= 0 &&
                ypos >= 0)
            {
                return _normals[xpos, ypos];
            }
            return new Vector3(0, 1, 0);
        }

        /* 
         * private float clacHeight(Texture2D pTex,int x, int y)
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
        /*
                public int getWidth()
                {
                    return m_HeightMap.Width;
                }
            }

                #endregion
            */
    }
}
//#endregion PUBLIC METHODS
