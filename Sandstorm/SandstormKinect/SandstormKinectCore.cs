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
        private DepthImagePixel[] m_DepthPixels, m_CustomDepthPixels;
        private byte[] m_ColorPixels;
        private Tuple<int, int> m_Startpoint, m_Dimension;

        public KinectProperties KinectProperties { get; set; }


        private GraphicsDevice m_gd;

        //misc
        Boolean m_ToggleCustomization;

        #endregion

        #region PROPPERTIES

        /// <summary>
        /// get RAW DepthPixels from KinectCamera
        /// </summary>
        public DepthImagePixel[] DepthPixels
        {
            get { return m_DepthPixels; }
            private set { m_DepthPixels = value; }
        }

        /// <summary>
        /// get RAW ColorPixels from KinectCamera
        /// </summary>
        public byte[] ColorPixels
        {
            get { return m_ColorPixels; }
            private set { m_ColorPixels = value;  }
        }

        /// <summary>
        /// get customized DepthImage (custom in size)
        /// </summary>
        public DepthImagePixel[] CustomDepthPixels
        {
            get { return m_CustomDepthPixels; }
            private set { m_CustomDepthPixels = value; }
        }

        /// <summary>
        /// flag to Enable/Disable Events for CustomDepthPixels 
        /// </summary>
        public Boolean ToggleCustomization
        {
            get { return m_ToggleCustomization; }
            set { m_ToggleCustomization = value; }
        }

        /// <summary>
        /// gets or sets the Startpoint for DepthImage cropping
        /// </summary>
        public Tuple<int, int> Startpoint
        {
            get { return m_Startpoint ?? (m_Startpoint = new Tuple<int, int>(0, 0)); }
            set { m_Startpoint = value; }
        }

        /// <summary>
        /// gets or sets the Dimension of CustomDepthImage, should be smaller than origin resolution
        /// </summary>
        public Tuple<int, int> Dimension
        {
            get { return m_Dimension ?? (m_Startpoint = new Tuple<int, int>(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight)); }
            set { m_Dimension = value; }
        }

        #endregion



        public SandstormKinectCore(GraphicsDevice gd)
        {
            m_gd = gd;
            KinectProperties = KinectProperties.Default;
        }
        /// <summary>
        /// Start the Kinect Camera
        /// </summary>
        public void StartKinect()
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

                //start thread
                if (this.sensor.DepthStream.IsEnabled)
                {
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
                  /*  this.sensor.Dispose();*/
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
            //bool firstFlag = true;
            bool depthValid = false;

            double diffThreshold = 10; //(this.sensor.DepthStream.FrameWidth*this.sensor.DepthStream.FrameHeight) * 5;
            short[] myDepthArray = new short[this.sensor.DepthStream.FrameWidth * this.sensor.DepthStream.FrameHeight];
            short[] myPrevDepthArray = new short[this.sensor.DepthStream.FrameWidth * this.sensor.DepthStream.FrameHeight];
  
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
                        for (int i=0; i< this.DepthPixels.Count(); i++)
                        {
                            //if (firstFlag)
                            //{
                            //    myPrevDepthArray[i] = this.DepthPixels[i].Depth;
                            //    myDepthArray[i] = this.DepthPixels[i].Depth;
                            //    firstFlag = false;
                            //}
                            //else
                            //{
                                myPrevDepthArray[i] = myDepthArray[i];
                                myDepthArray[i] = this.DepthPixels[i].Depth;
                                myDiffSum += Math.Abs( (double)myPrevDepthArray[i] - (double)myDepthArray[i]);

                            //}
                        }

                        /*for (int y = Bounds.Y; y < Bounds.Y + Bounds.Height; y++)
                        {
                            for (int x = Bounds.X, idx = 0; x < Bounds.X + Bounds.Width; x++, idx++)
                            {
                                myDepthArray[idx] = this.DepthPixels[y * this.sensor.DepthStream.FrameWidth + x].Depth;
                                myDiffSum += Math.Abs((double)myPrevDepthArray[idx] - (double)myDepthArray[idx]);
                            }
                        }

                        //
                        myDepthArray.CopyTo(myPrevDepthArray, 0);*/

                        //send event for changed depth image
                        if (this.SandstormKinectDepth != null && (myDiffSum / (640 * 480)) > diffThreshold)
                        {
                            this.SandstormKinectDepth(this, new SandstormKinectEvent(m_gd, myDepthArray, this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, KinectProperties));
                            Debug.WriteLine("event sent, diff-operator = {0}", (Math.Abs(myDiffSum)/ (640*480)));
                        }

                        depthValid = false;
                     //   System.Threading.Thread.Sleep(1000);
                    }

                    //fire event 
                    
                }
            }
            catch (ThreadAbortException ax)
            {
                Debug.WriteLine("GrabDepthFrameThread Aborted {0}",ax);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GrabDepthFrameThread Error {0}", ex);
            }
        
            
        }


        /// <summary>
        /// Image cropping
        /// </summary>
        /// <param name="_source">source Image</param>
        /// <param name="_destination">target / destination Image</param>
        /// <param name="_startpoint">Koordinates of Startpoint (int _x, int _y) </param>
        /// <param name="_dimension">target Dimensions (int _width, int _height)</param>
        internal void CropImage(DepthImagePixel _source, DepthImagePixel _destination, Tuple<int,int> _startpoint , Tuple<int, int> _dimension)
        {
            throw new NotImplementedException();
        }

    }
}
