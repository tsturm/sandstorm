using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace Sandstorm
{
    public class Kinect
    {
        KinectSensor sensor;
        bool ready = false;
        private DepthImagePixel[] depthPixels;
        private Microsoft.Xna.Framework.Vector4[] m_data;

        public Microsoft.Xna.Framework.Vector4[] data
        {
            get
            {
                if (ready)
                {
                    return m_data;
                }
                else return null;
            }

        }

        internal void StartKinect()
        {
            m_data = new Microsoft.Xna.Framework.Vector4[420*420];
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
                this.depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];

                this.sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);

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

        internal void StopKinect()
        {
            if (null != this.sensor)
            {
                this.sensor.DepthFrameReady -= new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
                //this.sensor.Stop();
                this.sensor.Dispose();
            }

        }

        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            ready = false;

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

                    int minDepth = depthFrame.MinDepth;
                    int maxDepth = depthFrame.MaxDepth;
                    short myActualPixel;

                    int width = 640;
                    int height = 480;
                    int idx = 0;

                    for (int y = 30; y < height-30; y++)
                    {
                        for (int x = 110; x < width-110; x++)
                        {
                            myActualPixel = (short)(this.depthPixels[y * width + x].Depth - 1000);
                            if (myActualPixel > 0 && myActualPixel <= 230)
                            {
                                //validen Bildpunkt gefunden
                                m_data[idx].X = (float)-(((float)myActualPixel / 230f) - 1);
                                m_data[idx].Y = (float)-(((float)myActualPixel / 230f) - 1);
                                m_data[idx].Z = (float)-(((float)myActualPixel / 230f) - 1);
                                m_data[idx].W = 1f;
                            }
                            else
                            {
                                m_data[idx].X = 0f;
                                m_data[idx].Y = 0f;
                                m_data[idx].Z = 0f;
                                m_data[idx].W = 1f;
                            }

                            idx++;
                        }
                    }
                    ready = true;
                }
            }
        }

        
    }
}
