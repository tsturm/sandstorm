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

        #endregion

        #region PROPPERTIES

        public short[] DepthImage
        {
            get { return m_DepthImage; }
            set { m_DepthImage = value; }
        }
        
        #endregion

        #region CONSTRUCTOR

        public SandstormKinectEvent(short[] _image) { this.DepthImage = _image; }

        #endregion
    }
}
