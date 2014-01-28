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
        public static readonly CameraProperties Default = new CameraProperties()
        {
            CameraMode = CAMERA_MODE.CAMERA_MODE_TURNTABLE,
            ProjectionType = ProjectionType.PERSPECTIVE_PROJECTION,
            ViewMatrix = Matrix.Identity,
            ProjectionMatrix = Matrix.Identity,
            CameraName = "Perspective"
        };

        public static readonly CameraProperties DefaultOrtho = new CameraProperties()
        {
                CameraMode = CAMERA_MODE.CAMERA_MODE_TURNTABLE,
                ProjectionType = ProjectionType.ORTHOGRAPHIC_PROJECTION,
                ViewMatrix = Matrix.Identity,
                ProjectionMatrix = Matrix.Identity,
                CameraName = "Orthografic"
        };

        /// <summary>
        /// ViewMatrix for Camera mapping
        /// </summary>
        /// 
        public Matrix _viewMatrix;
        public Matrix ViewMatrix { get { return _viewMatrix; } set { _viewMatrix = value; } }

        /// <summary>
        /// ProjectionMatrix for Camera mapping
        /// </summary>
        /// 
        public Matrix _projMatrix;
        public Matrix ProjectionMatrix { get { return _projMatrix; } set { _projMatrix = value; } }


        /// <summary>
        /// ProjectionType for Camera Type
        /// </summary>
        /// 
        public ProjectionType ProjectionType { get ; set; }


        /// <summary>
        /// CameraMode for Camera Mode
        /// </summary>
        /// 
        public CAMERA_MODE CameraMode { get; set; }


        /// <summary>
        /// CameraName for Camera Name
        /// </summary>
        /// 
        public String CameraName { get; set; }

        public CameraProperties()
        {
            ProjectionType = ProjectionType.PERSPECTIVE_PROJECTION;
            CameraMode = CAMERA_MODE.CAMERA_MODE_TURNTABLE;
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;
        }




    }
}
