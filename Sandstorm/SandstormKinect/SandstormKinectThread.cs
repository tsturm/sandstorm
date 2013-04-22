using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;

namespace SandstormKinect
{
    public class SandstormKinectThread
    {

        #region FIELDS

        KinectSensor sensor;
        private DepthImagePixel[] m_DepthPixels;
        private byte[] m_ColorPixels;

        #endregion

        #region PROPPERTIES

        /// <summary>
        /// get RAW DepthPixels from KinectCamera
        /// </summary>
        public DepthImagePixel[] DepthPixels
        {
            get { return m_DepthPixels; }
        }

        /// <summary>
        /// get RAW ColorPixels from KinectCamera
        /// </summary>
        public byte[] ColorPixels
        {
            get { return m_ColorPixels; }
        }

        #endregion

        /// <summary>
        /// Start the Kinect Camera
        /// </summary>
        public void StartKinect()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                this.sensor.DepthStream.Range = DepthRange.Near;
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);


                this.m_DepthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];
                this.m_ColorPixels = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }
        }


        /// <summary>
        /// Stop the Kinect Camera
        /// </summary>
        public void StopKinect()
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
                this.sensor.Dispose();
            }
        }


        /// <summary>
        /// Start the Thread who receives the DepthImage an process the basic image
        /// </summary>
        internal void StartDepthImage_Thread()
        {

        }
    }
}
