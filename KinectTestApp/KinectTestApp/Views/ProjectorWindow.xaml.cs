using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestApp.ViewModels;

namespace TestApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class ProjectorWindow : Window
    {
        public ProjectorWindowViewModel ViewModel
        {
            get { return this.DataContext as ProjectorWindowViewModel; }
            set { this.DataContext = value; }
        }

        public ProjectorWindow()
        {
            InitializeComponent();
            this.KeyDown += RootKey;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ViewModel.Close();
        }

        void RootKey(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Escape) || (e.Key == Key.F11))
            {
                if (this.WindowStyle == WindowStyle.None)
                {
                    this.WindowStyle = WindowStyle.ThreeDBorderWindow;
                    this.WindowState = WindowState.Normal;
                }
            }

            e.Handled = true;
        }

    }
}
