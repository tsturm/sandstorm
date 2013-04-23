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
    public class ProjectorWindowViewModel : ObservableObject
    {
        #region FIELDS

        ICommand m_StartKinectCommand;

        #endregion

        #region PROPERTIES
   
        public ICommand StartKinectCommand
        {
            get { return m_StartKinectCommand ?? (m_StartKinectCommand = new RelayCommand(this.StartKinect)) ;}
        }

        #endregion

        #region COMMANDS
        
        internal void StartKinect()
        {
            throw new NotImplementedException();
        }

        #endregion

        internal void Close()
        {
            throw new NotImplementedException();
        }
    }
}
