using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using TestApp.Infrastructure;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Windows.Input;
using System.Windows;

namespace TestApp.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        #region FIELDS
        // hier kommt die ViewModel Implementation hin
        KinectSensor  sensor;

        ProjectorWindow m_ProjectorWindow;

        private WriteableBitmap colorBitmap;
        private DepthImagePixel[] depthPixels;
        private byte[] colorPixels;

        private bool m_Status = false;
        private WriteableBitmap m_Image;
        private int m_AvgCenterDepth = -1;
        private bool m_ImageSelector = false; //0 == Color, 1 == Depth

        ICommand m_StartKinectCommand;
        ICommand m_StopKinectCommand;
        ICommand m_SetColor;
        ICommand m_SetDepth;
        ICommand m_OpenProjectorWindowCommand;
        ICommand m_CloseProjectorWindowCommand;

        #endregion

        #region PROPERTIES
        public WriteableBitmap Image
        {
            get { return m_Image ?? (m_Image = new WriteableBitmap(640, 480, 96.0, 96.0, PixelFormats.Bgr32, null)); }
            set { m_Image = value; RaisePropertyChanged("Image"); }
        }

        public bool Status
        {
            get { return m_Status;}
            set { m_Status = value; RaisePropertyChanged("Status"); }
        }

        public bool ImageSelector
        {
            get { return m_ImageSelector; }
            set { m_ImageSelector = value; RaisePropertyChanged("ImageSelector"); }
        }

        public int AvgCenterDepth
        {
            get { return m_AvgCenterDepth; }
            set { m_AvgCenterDepth = value; RaisePropertyChanged("AvgCenterDepth"); }
        }

        public ICommand StartKinectCommand
        {
            get { return m_StartKinectCommand ?? (m_StartKinectCommand = new RelayCommand(this.StartKinect)) ;}
        }

        public ICommand StopKinectCommand
        {
            get { return m_StopKinectCommand ?? (m_StopKinectCommand = new RelayCommand(this.StopKinect)); }
        }

        public ICommand SetColor
        {
            get { return m_SetColor ?? (m_SetColor = new RelayCommand(this.SetColorImage)); }
        }

        public ICommand SetDepth
        {
            get { return m_SetDepth ?? (m_SetDepth = new RelayCommand(this.SetDepthImage)); }
        }

        public ICommand OpenProjectorWindowCommand
        {
            get { return m_OpenProjectorWindowCommand ?? (m_OpenProjectorWindowCommand = new RelayCommand(this.OpenProjector)); }
        }

        public ICommand CloseProjectorWindowCommand
        {
            get { return m_CloseProjectorWindowCommand ?? (m_CloseProjectorWindowCommand = new RelayCommand(this.CloseProjector)); }
        }

        #endregion

        #region COMMANDS

        internal void StartKinect()
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

                
                this.depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];
                this.colorPixels = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];
                this.colorBitmap = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
                
                this.Image = this.colorBitmap;
                //register handler
                this.sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);

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

            if (null == this.sensor)
            {
                Status = false;
                Image = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
            }
            else
            {
                Status = true;
            }
        }

        internal void StopKinect()
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
                this.Image = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
                Status = false;
            }

        }

        internal void OpenProjector()
        {
            m_ProjectorWindow = new ProjectorWindow { ViewModel = new ViewModels.ProjectorWindowViewModel() };
            m_ProjectorWindow.Show();
        }

        internal void CloseProjector()
        {
            m_ProjectorWindow.Close();
            m_ProjectorWindow = null;
        }

        internal void SetColorImage() { ImageSelector = false; }
        internal void SetDepthImage() { ImageSelector = true; }

        #endregion

        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (!ImageSelector)
            {
                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                {
                    if (colorFrame != null)
                    {
                        colorFrame.CopyPixelDataTo(this.colorPixels);

                        this.Image.WritePixels(
                            new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                            this.colorPixels,
                            this.colorBitmap.PixelWidth * sizeof(int),
                            0);
                    }
                }
            }

            if (ImageSelector)
            {
                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    if (depthFrame != null)
                    {
                        depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

                        int minDepth = depthFrame.MinDepth;
                        int maxDepth = depthFrame.MaxDepth;

                        int colorPixelIndex = 0;
                        for (int i = 0; i < this.depthPixels.Length; ++i)
                        {
                            short depth = depthPixels[i].Depth;

                            byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                            this.colorPixels[colorPixelIndex++] = intensity;
                            this.colorPixels[colorPixelIndex++] = intensity;
                            this.colorPixels[colorPixelIndex++] = intensity;
                            ++colorPixelIndex;
                        }

                        this.Image.WritePixels(
                            new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                            this.colorPixels,
                            this.colorBitmap.PixelWidth * sizeof(int),
                            0);

                        if (depthPixels != null && ImageSelector)
                        {
                            AvgCenterDepth = (depthPixels[153278].Depth + depthPixels[153280].Depth) / 2 ;
                        }
                        else AvgCenterDepth = -1;
                    }
                }
            }


        }

        internal void Close()
        {
            throw new NotImplementedException();
        }
    }
}
