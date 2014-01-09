using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandstormKinect
{
    public class KinectProperties
    {
        public static readonly KinectProperties Default = new KinectProperties() { };

        /// <summary>
        /// 
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Height { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public float MinDistance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float MaxDistance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public KinectProperties()
        {
            Width = 420;
            Height = 420;
            MinDistance = 1025.0f;
            MaxDistance = 1225.0f - MinDistance;
        }
    }
}
