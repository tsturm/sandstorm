using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace TestApp
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var myWindow = new MainWindow { ViewModel = new ViewModels.MainWindowViewModel() };
               
                myWindow.Show();
            }
            catch (Exception ex)
            {
                string message = string.Format("Exception: {0} \n\nStack: {1}", ex.Message, ex.StackTrace);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
