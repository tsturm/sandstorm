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

        #endregion

        public SandstormKinectEvent(Vector4[] data)
        {
            this.TextureData = data;
        }

        public SandstormKinectEvent(Texture2D tex)
        {
            this.Texture = tex;
        }

        public SandstormKinectEvent()
        {
            //simple polling event
        }
    }
}
