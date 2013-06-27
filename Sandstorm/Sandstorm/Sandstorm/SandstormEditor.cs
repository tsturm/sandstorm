using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Sandstorm
{
    public partial class SandstormEditor : Form
    {
        private Viewport _viewPort;

        public Viewport ViewPort
        {
            get { return _viewPort; }
            set { _viewPort = value; }
        }

        public SandstormEditor()
        {
            InitializeComponent();
            //CheckForIllegalCrossThreadCalls = false;
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            panel1.Focus();
        }

        private void panel1_Leave(object sender, EventArgs e)
        {
            Debug.WriteLine("noe");
        }
    }
}
