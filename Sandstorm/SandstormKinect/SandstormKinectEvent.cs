using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SandstormKinect
{
    /// <summary>
    /// Event wich contains DepthImage from Kinect
    /// </summary>
    public class SandstormKinectEvent : EventArgs
    {

        #region OLD_Stuff

        /*
        #region FIELDS

        private short[] m_DepthImage;
        private int m_Width, m_Height;
        private Texture2D m_texture;

        #endregion

        #region PROPPERTIES

        public short[] DepthImage
        {
            get { return m_DepthImage; }
            set { m_DepthImage = value; }
        }

        public int Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        public int Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        public Texture2D Texture
        {
            get { return m_texture; }
            set { m_texture = value; }
        }

        #endregion

        #region CONSTRUCTOR

        public SandstormKinectEvent(GraphicsDevice gd, short[] _image, int _width, int _height, KinectProperties kp)
        { 
            this.DepthImage = _image;
            this.Width = _width;
            this.Height = _height;

            m_texture = new Texture2D(gd, kp.Width, kp.Height, false, SurfaceFormat.Vector4);

            Vector4[] data = new Vector4[kp.Width * kp.Height];

            int max = 0;
            int min = Int32.MaxValue;

            int startX = (Width - kp.Width) / 2;
            int startY = (Height - kp.Height) / 2;

            for (int y = startY; y < startY + kp.Height; y++)
            {
                for (int x = startX; x < startX + kp.Width; x++)
                {
                    int index = y * Width + x;

                    float value = 1.0f - ((float)(_image[index] - kp.MinDistance) / kp.MaxDistance);

                    value = Math.Min(1, value);
                    value = Math.Max(0, value);
                    
                    int index2 = ((startY + kp.Height - 1) - (y - startY) - startY)*kp.Width + (x - startX);

                    data[index2].X = value;
                    data[index2].Y = value;
                    data[index2].Z = value;
                    data[index2].W = 1.0f;



                    max = Math.Max(_image[index], max);
                    if(_image[index] != 0)
                    min = Math.Min(_image[index], min);
                }
            }
            
            m_texture.SetData(data);
        }

        #endregion
        */
        #endregion


        #region EVENT_PROPERTIES

        public Microsoft.Xna.Framework.Vector4[] TextureData
        {
            get;
            set;
        }

        public Texture2D Texture
        {
            get;
            set;
        }

        public Texture2D Texture2
        {
            get;
            set;
        }

        #endregion

        public SandstormKinectEvent(Texture2D tex)
        {
            //set data
            this.Texture = tex;
            this.Texture2 = tex;
        }
    }
}
