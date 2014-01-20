using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Sandstorm.Navigation
{
    [Serializable]
    [XmlRootAttribute("CameraProperties", Namespace = "sandstorm.h-da.de", IsNullable = false)]

    public class CameraProperties
    {
        /// <summary>
        /// ViewMatrix for Camera mapping
        /// </summary>
        public Matrix ViewMatrix { get; set; }

        /// <summary>
        /// ProjectionMatrix for Camera mapping
        /// </summary>
        public Matrix ProjectionMatrix { get; set; }

        public static readonly CameraProperties Default = new CameraProperties() 
        {
            ViewMatrix = Matrix.Identity,
            ProjectionMatrix = Matrix.Identity
        };


    }
}
