﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Serialization;

namespace ParticleStormDLL
{
    [Serializable()]
    [XmlRootAttribute("ParticleProperties", Namespace = "sandstorm.h-da.de", IsNullable = false)]
    public class ParticleProperties
    {
        /// <summary>
        /// A built-in state object with default properties for a Particle.
        /// </summary>
        public static readonly ParticleProperties Default = new ParticleProperties() {};

        /// <summary>
        /// Gets or sets the minimal life time of a particle in seconds
        /// </summary>
        public float LifeTimeMin { get; set; }

        /// <summary>
        /// Gets or sets the maximal life time of a particle in seconds
        /// </summary>
        public float LifeTimeMax { get; set; }

        /// <summary>
        /// Gets or sets the minimal position of a particle
        /// </summary>
        public Vector3 PositionMin { get; set; }

        /// <summary>
        /// Gets or sets the maximal position of a particle
        /// </summary>
        public Vector3 PositionMax { get; set; }

        /// <summary>
        /// Gets or sets the minimal velocity of a particle
        /// </summary>
        public Vector3 VelocityMin { get; set; }

        /// <summary>
        /// Gets or sets the maximal velocity of a particle
        /// </summary>
        public Vector3 VelocityMax { get; set; }

        /// <summary>
        /// Gets or sets the minimal start size of a particle
        /// </summary>
        public float StartSizeMin { get; set; }

        /// <summary>
        /// Gets or sets the maximal start size of a particle
        /// </summary>
        public float StartSizeMax { get; set; }

        /// <summary>
        /// Gets or sets the minimal end size of a particle
        /// </summary>
        public float EndSizeMin { get; set; }

        /// <summary>
        /// Gets or sets the maximal end size of a particle
        /// </summary>
        public float EndSizeMax { get; set; }

        /// <summary>
        /// Gets or sets the minimal start color of a particle
        /// </summary>
        public Color StartColorMin { get; set; }

        /// <summary>
        /// Gets or sets the maximal start color of a particle
        /// </summary>
        public Color StartColorMax { get; set; }

        /// <summary>
        /// Gets or sets the minimal end color of a particle
        /// </summary>
        public Color EndColorMin { get; set; }

        /// <summary>
        /// Gets or sets the maximal end color of a particle
        /// </summary>
        public Color EndColorMax { get; set; }

        /// <summary>
        /// Gets or Sets the path of the texture file for a particle
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Enables or disables additive blending of a particle. The default is false.
        /// </summary>
        public bool AdditiveBlending { get; set; }

        /// <summary>
        /// Creates an instance of ParticleProperties with default values.
        /// </summary>
        public ParticleProperties()
        {
            LifeTimeMin = 5.0f;
            LifeTimeMax = 30.0f;
            PositionMin = new Vector3(-100, 200, -100);
            PositionMax = new Vector3(100, 200, 100);
            VelocityMin = new Vector3(-30, 0, -20);
            VelocityMax = new Vector3(30, 0, 20);
            StartSizeMin = 1.0f;
            StartSizeMax = 5.0f;
            EndSizeMin = 0.0001f;
            EndSizeMax = 0.0001f;
            StartColorMin = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            StartColorMax = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            EndColorMin = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            EndColorMax = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            Texture = null;
            AdditiveBlending = true;
        }
    }
}
