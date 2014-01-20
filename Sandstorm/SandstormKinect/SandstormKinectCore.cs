﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace SandstormKinect
{
    public class SandstormKinectCore
    {

        #region FIELDS

        KinectSensor sensor;

        //event
        public event EventHandler<SandstormKinectEvent> SandstormKinectDepth;

        //threads
        private Thread m_GrabDepthFrameThread;

        //images
        private DepthImagePixel[] m_DepthPixels; //, m_CustomDepthPixels;
        //private byte[] m_ColorPixels;
        //private Tuple<int, int> m_Startpoint, m_Dimension;

        //public KinectProperties KinectProperties { get; set; }


        //private GraphicsDevice m_gd;

        //misc
        //Boolean m_ToggleCustomization;
        KinectProperties m_KinectSettings;

        //private Microsoft.Xna.Framework.Vector4[] m_TextureData;
        private Object thisLock = new Object();

        #endregion

        #region PROPPERTIES

        private GraphicsDevice GraphicsDevice { get; set; }
        private Texture2D TextureKinect { get; set; }

        public KinectProperties KinectSettings
        {
            get { return m_KinectSettings; }
            set { m_KinectSettings = value; }
        }

        /// <summary>
        /// get RAW DepthPixels from KinectCamera
        /// </summary>
        public DepthImagePixel[] DepthPixels
        {
            get { return m_DepthPixels; }
            private set { m_DepthPixels = value; }
        }

        //public Microsoft.Xna.Framework.Vector4[] TextureData
        //{
        //    get { return m_TextureData ?? (m_TextureData = new Microsoft.Xna.Framework.Vector4[this.KinectSettings.TargetDimension.Item1 * this.KinectSettings.TargetDimension.Item2]); }
        //    set { m_TextureData = value; }
        //}
        #region old_stuff
        /// <summary>
        /// get RAW ColorPixels from KinectCamera
        /// </summary>
        //public byte[] ColorPixels
        //{
        //    get { return m_ColorPixels; }
        //    private set { m_ColorPixels = value;  }
        //}

        /// <summary>
        /// get customized DepthImage (custom in size)
        /// </summary>
        //public DepthImagePixel[] CustomDepthPixels
        //{
        //    get { return m_CustomDepthPixels; }
        //    private set { m_CustomDepthPixels = value; }
        //}

        /// <summary>
        /// flag to Enable/Disable Events for CustomDepthPixels 
        /// </summary>
        //public Boolean ToggleCustomization
        //{
        //    get { return m_ToggleCustomization; }
        //    set { m_ToggleCustomization = value; }
        //}
        #endregion

        #endregion


        /// <summary>
        /// basic constructor
        /// </summary>
        public SandstormKinectCore()
        {
            //KinectSettings = KinectProperties.Sandstorm;
        }

        /// <summary>
        /// Start the Kinect Camera
        /// </summary>
        public void StartKinect() //GraphicsDevice graphicsDevice, Texture2D tex)
        {
            try
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
                    //this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                    this.m_DepthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];
                    //this.m_ColorPixels = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];

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

                //start thread
                if (this.sensor.DepthStream.IsEnabled)
                {
                    //link Graphicsdevice
                    //this.GraphicsDevice = graphicsDevice;
                    //link texture
                    //this.TextureKinect = tex;

                    //build DepthStream Thread
                    if (m_GrabDepthFrameThread != null)
                    {
                        m_GrabDepthFrameThread.Abort();
                        m_GrabDepthFrameThread.Join();
                    }
                    m_GrabDepthFrameThread = new Thread(new ThreadStart(this.DepthImage_Thread));
                    m_GrabDepthFrameThread.Name = "GrabDepthFrameWorkerThread";

                    //go go go
                    m_GrabDepthFrameThread.Start();

                    //Debug thread status
                    if (m_GrabDepthFrameThread.IsAlive)
                    {
                        Debug.WriteLine("thread is alive");
                    }
                    else Debug.WriteLine("something gone wrong with thread!");
                }
            
            }
            catch (Exception ex)
            {
                Debug.WriteLine("catch e: " + ex.Message);
            }
            
        }

        /// <summary>
        /// Stop the Kinect Camera
        /// </summary>
        public void StopKinect()
        {
            try
            {
                if (null != this.sensor)
                {

                    m_GrabDepthFrameThread.Abort();
                    this.sensor.Stop();
                    /* this.sensor.Dispose(); */
                }
            }
            catch (ThreadAbortException ex)
            {
                Debug.WriteLine("GrabDepthFrameThread Error {0}", ex);
            }
        }

        /// <summary>
        /// Thread who receives the DepthImage an process the basic image
        /// </summary>
        private void DepthImage_Thread()
        {

            bool firstFlag = true;  //need to check the first frame, fro later comparision
            bool depthValid = false; //if a valid depth frame was received

            short[] myDepthArray = new short[this.sensor.DepthStream.FrameWidth * this.sensor.DepthStream.FrameHeight];
            short[] myPrevDepthArray = new short[this.sensor.DepthStream.FrameWidth * this.sensor.DepthStream.FrameHeight];

            //textureData
            Microsoft.Xna.Framework.Vector4[] TextureData = new Microsoft.Xna.Framework.Vector4[this.KinectSettings.TargetDimension.Item1 * this.KinectSettings.TargetDimension.Item2];
            //Texture2D myTexture = new Texture2D(this.GraphicsDevice, this.KinectSettings.TargetDimension.Item1, this.KinectSettings.TargetDimension.Item2, false, SurfaceFormat.Vector4);

            //borders
            // -> KinectImage / CroppedImage ist based on ImageCenter
            int yBorderTarget = (this.sensor.DepthStream.FrameHeight / 2) - (this.KinectSettings.TargetDimension.Item2 / 2) + this.KinectSettings.TargetDimension.Item2;
            int yBorderBegin = (this.sensor.DepthStream.FrameHeight / 2) - (this.KinectSettings.TargetDimension.Item2 / 2);
            int xBorderTarget = (this.sensor.DepthStream.FrameWidth / 2) - (this.KinectSettings.TargetDimension.Item1 / 2) + this.KinectSettings.TargetDimension.Item1;
            int xBorderBegin = (this.sensor.DepthStream.FrameWidth / 2) - (this.KinectSettings.TargetDimension.Item1 / 2);
            
            //misc
            float value;
            

            try
            {
                while (true)
                {
                    depthValid = false;
                    double myDiffSum = 0;

                    using (DepthImageFrame depthFrame = this.sensor.DepthStream.OpenNextFrame(0))
                    {
                        if (depthFrame != null)
                        {
                            depthFrame.CopyDepthImagePixelDataTo(this.DepthPixels);
                            depthValid = true;
                        }
                    }

                    if (depthValid)
                    {
                        for (int i = 0; i < this.DepthPixels.Count(); i++)
                        {
                            if (firstFlag)
                            {
                                myPrevDepthArray[i] = this.DepthPixels[i].Depth;
                                myDepthArray[i] = this.DepthPixels[i].Depth;
                                firstFlag = false;
                            }
                            else
                            {
                                myPrevDepthArray[i] = myDepthArray[i];
                                myDepthArray[i] = this.DepthPixels[i].Depth;
                                myDiffSum += Math.Abs((double)myPrevDepthArray[i] - (double)myDepthArray[i]);
                            }
                        }
                        //make act. Depths to new prev. Depths
                        myDepthArray.CopyTo(myPrevDepthArray, 0);

                        //look for changes within DepthImage
                        if (this.SandstormKinectDepth != null && (myDiffSum / this.DepthPixels.Count()) > this.KinectSettings.DiffThreshold)
                        {
                            //Debug.WriteLine("KinectCore", "calc new DepthData");
                            //Debug.WriteLine("KinectCore, diff-operator = {0}", (myDiffSum / this.DepthPixels.Count()));
                            //create new Depth Texture
                            for (int y = yBorderBegin, idy = this.KinectSettings.TargetDimension.Item2 - 1; y < yBorderTarget; y++, idy--)
                            {
                                for (int x = xBorderBegin, idx = 0; x < xBorderTarget; x++, idx++)
                                {
                                    short depth = myDepthArray[y * this.sensor.DepthStream.FrameWidth + x];
                                    
                                    //todo -> Normierung
                                    if (depth > this.KinectSettings.NearLevelDistance)
                                    {
                                        value = 1.0f - ((depth - this.KinectSettings.NearLevelDistance) / (this.KinectSettings.FarLevelDistance - this.KinectSettings.NearLevelDistance));
                                        value = Math.Min(1, value);
                                        value = Math.Max(0, value);
                                    } else value = 1.0f;
                                    TextureData[idy * this.KinectSettings.TargetDimension.Item1 + idx] = new Microsoft.Xna.Framework.Vector4(value, value, value, 1.0f);
                                 }
                            }

                            //Set Data for Event
                            //this.TextureKinect.SetData(TextureData);
                            //this.TextureKinect.Name = "kinect";
   
                            //lock (thisLock)
                            //{
                                //fire Event
                                //SandstormKinectEvent ev = new SandstormKinectEvent(myTexture);
                                this.SandstormKinectDepth(this, new SandstormKinectEvent(TextureData));
                            //}
                            //wait a bit
                            //Thread.Sleep(250);
                        }
                        depthValid = false;
                    }

                }
            }
            catch (ThreadAbortException ax)
            {
                Debug.WriteLine("GrabDepthFrameThread Aborted {0}", ax);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GrabDepthFrameThread Error {0}", ex);
            }
        }
    }
}
