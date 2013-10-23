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
using Microsoft.Xna.Framework;

namespace Sandstorm
{
    public partial class SandstormEditor : Form
    {
        private Viewport _viewPort;
        private float _fps = 0;
        private int _particles = 0;

        public event EventHandler<TerrainArgs> TerrainHeightChanged;
        public event EventHandler<TerrainArgs> TerrainColorChanged;
        public event EventHandler<TerrainArgs> TerrainContoursChanged;

        public Viewport ViewPort
        {
            get { return _viewPort; }
            set { _viewPort = value; }
        }

        public float FPS
        {
            get { return _fps; }
            set { FPSLabel.Text = "FPS: " + value.ToString(); }
        }

        public int Particles
        {
            get { return _particles; }
            set { ParticleLabel.Text = "Particles: " + value.ToString(); }
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

        private void terrainHeight_ValueChanged(object sender, EventArgs e)
        {
            // make a copy to be more thread-safe
            EventHandler<TerrainArgs> handler = TerrainHeightChanged;

            if (handler != null)
            {
                TerrainArgs args = new TerrainArgs();
                args.Height = Convert.ToInt32(terrainHeight.Value);
                handler(this, args);
            }
        }

        private void lowColor_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();

            dialog.ShowDialog();

            EventHandler<TerrainArgs> handler = TerrainColorChanged;

            if (handler != null)
            {
                TerrainArgs args = new TerrainArgs();
                args.Height = Convert.ToInt32(terrainHeight.Value);

                color0.FillColor = dialog.Color;
                args.Color0 = new Vector4(color0.FillColor.R / 255.0f, color0.FillColor.G / 255.0f, color0.FillColor.B / 255.0f, 1.0f);
                args.Color1 = new Vector4(color1.FillColor.R / 255.0f, color1.FillColor.G / 255.0f, color1.FillColor.B / 255.0f, 1.0f);
                args.Color2 = new Vector4(color2.FillColor.R / 255.0f, color2.FillColor.G / 255.0f, color2.FillColor.B / 255.0f, 1.0f);
                args.Color3 = new Vector4(color3.FillColor.R / 255.0f, color3.FillColor.G / 255.0f, color3.FillColor.B / 255.0f, 1.0f);

                handler(this, args);
            }
        }

        private void color1_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();

            dialog.ShowDialog();

            EventHandler<TerrainArgs> handler = TerrainColorChanged;

            if (handler != null)
            {
                TerrainArgs args = new TerrainArgs();
                args.Height = Convert.ToInt32(terrainHeight.Value);

                color1.FillColor = dialog.Color;
                args.Color0 = new Vector4(color0.FillColor.R / 255.0f, color0.FillColor.G / 255.0f, color0.FillColor.B / 255.0f, 1.0f);
                args.Color1 = new Vector4(color1.FillColor.R / 255.0f, color1.FillColor.G / 255.0f, color1.FillColor.B / 255.0f, 1.0f);
                args.Color2 = new Vector4(color2.FillColor.R / 255.0f, color2.FillColor.G / 255.0f, color2.FillColor.B / 255.0f, 1.0f);
                args.Color3 = new Vector4(color3.FillColor.R / 255.0f, color3.FillColor.G / 255.0f, color3.FillColor.B / 255.0f, 1.0f);

                handler(this, args);
            }
        }

        private void color2_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();

            dialog.ShowDialog();

            EventHandler<TerrainArgs> handler = TerrainColorChanged;

            if (handler != null)
            {
                TerrainArgs args = new TerrainArgs();
                args.Height = Convert.ToInt32(terrainHeight.Value);

                color2.FillColor = dialog.Color;
                args.Color0 = new Vector4(color0.FillColor.R / 255.0f, color0.FillColor.G / 255.0f, color0.FillColor.B / 255.0f, 1.0f);
                args.Color1 = new Vector4(color1.FillColor.R / 255.0f, color1.FillColor.G / 255.0f, color1.FillColor.B / 255.0f, 1.0f);
                args.Color2 = new Vector4(color2.FillColor.R / 255.0f, color2.FillColor.G / 255.0f, color2.FillColor.B / 255.0f, 1.0f);
                args.Color3 = new Vector4(color3.FillColor.R / 255.0f, color3.FillColor.G / 255.0f, color3.FillColor.B / 255.0f, 1.0f);

                handler(this, args);
            }
        }

        private void color3_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();

            dialog.ShowDialog();

            EventHandler<TerrainArgs> handler = TerrainColorChanged;

            if (handler != null)
            {
                TerrainArgs args = new TerrainArgs();
                args.Height = Convert.ToInt32(terrainHeight.Value);

                color3.FillColor = dialog.Color;
                args.Color0 = new Vector4(color0.FillColor.R / 255.0f, color0.FillColor.G / 255.0f, color0.FillColor.B / 255.0f, 1.0f);
                args.Color1 = new Vector4(color1.FillColor.R / 255.0f, color1.FillColor.G / 255.0f, color1.FillColor.B / 255.0f, 1.0f);
                args.Color2 = new Vector4(color2.FillColor.R / 255.0f, color2.FillColor.G / 255.0f, color2.FillColor.B / 255.0f, 1.0f);
                args.Color3 = new Vector4(color3.FillColor.R / 255.0f, color3.FillColor.G / 255.0f, color3.FillColor.B / 255.0f, 1.0f);

                handler(this, args);
            }
        }

        private void contours_Changed(object sender, EventArgs e)
        {
            contourSpacing.Enabled = contours.Checked;

            EventHandler<TerrainArgs> handler = TerrainContoursChanged;

            if (handler != null)
            {
                TerrainArgs args = new TerrainArgs();
                args.DisplayContours = contours.Checked;
                args.ContourSpacing = (float)Convert.ToDouble(contourSpacing.Value);
                handler(this, args);
            }
        }
    }

    public class TerrainArgs : EventArgs
    {
        public int Height { get; set; }
        public bool DisplayContours { get; set; }
        public float ContourSpacing { get; set; }
        public Vector4 Color0 { get; set; }
        public Vector4 Color1 { get; set; }
        public Vector4 Color2 { get; set; }
        public Vector4 Color3 { get; set; }
    }
}
