using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandstormKinect
{
    /// <summary>
    /// Event wich contains DepthImage from Kinect
    /// </summary>
    public class SandstormKinectEvent : EventArgs
    {
        #region FIELDS

        private short[] m_DepthImage;
        private int m_Width, m_Height;

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

        #endregion

        #region CONSTRUCTOR

        public SandstormKinectEvent(short[] _image, int _width, int _height)
        { 
            this.DepthImage = _image;
            this.Width = _width;
            this.Height = _height;
        }

        #endregion
    }
}
