using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Sandstorm
{
    [Serializable]
    [XmlRootAttribute("TerrainProperties", Namespace = "sandstorm.h-da.de", IsNullable = false)]
    public class TerrainProperties
    {
        public static readonly TerrainProperties Default = new TerrainProperties() { };

        /// <summary>
        /// 
        /// </summary>
        public Vector4 Color0 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector4 Color1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector4 Color2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector4 Color3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ContourLines { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float ContourSpacing { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float HeightScale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TerrainProperties()
        {
            Color0 = new Vector4(0.0f, 0.0f, 0.85f, 1.0f);
            Color1 = new Vector4(0.2f, 0.52f, 0.03f, 1.0f);
            Color2 = new Vector4(0.9f, 0.85f, 0.34f, 1.0f);
            Color3 = new Vector4(0.7f, 0.17f, 0.0f, 1.0f);
            HeightScale = 84.0f;
            ContourSpacing = 35.0f;
            ContourLines = true;
        }
    }
}
