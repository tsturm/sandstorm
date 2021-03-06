﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using SandstormKinect.Util;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SandstormKinect
{
    [Serializable()]
    [XmlRootAttribute("KinectProperties", Namespace = "sandstorm.h-da.de", IsNullable = false)]
    public class KinectProperties
    {

        /// <summary>
        /// gets or sets the Startpoint for DepthImage cropping
        /// 
        /// </summary>
        public MyTuple<int, int> Startpoint { get; set; }

        /// <summary>
        /// gets or sets the Dimension of CustomDepthImage, should be smaller than origin resolution
        /// item1 width, item2 height
        /// </summary>
        public MyTuple<int, int> TargetDimension { get; set; }

        /// <summary>
        /// gets or sets the Offset, where the result is shown on Screen
        /// /// item1 x, item2 y
        /// </summary>
        public MyTuple<int, int> TargetOffset { get; set; }

        /// <summary>
        /// Threshold between old and new DepthImage
        /// </summary>
        public int DiffThreshold { get; set; }

        /// <summary>
        /// Wert, ab wo die Kinect etwas sieht
        /// </summary>
        public float NearLevelDistance { get; set; }

        /// <summary>
        /// Wert, wie weit die Kinect etwas sieht
        /// </summary>
        public float FarLevelDistance { get; set; }


        /// <summary>
        /// A built-in state object with default properties for a Particle.
        /// </summary>
        public static readonly KinectProperties Default = new KinectProperties()
        {
            Startpoint =  Tuple.Create(0, 0),
            TargetDimension = Tuple.Create(420, 420),
            DiffThreshold = 10,
            NearLevelDistance = 990.0f,
            FarLevelDistance = 1250.0f

        };

        public static readonly KinectProperties Sandstorm = new KinectProperties()
        {
            //sandstorm  standart values here
            Startpoint = Tuple.Create(100, 20),
            TargetDimension = Tuple.Create(420, 420),
            DiffThreshold = 5,
            NearLevelDistance = 970.0f,
            FarLevelDistance = 1260.0f
        };

    }
}