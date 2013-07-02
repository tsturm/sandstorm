namespace Sandstorm
{
    partial class SandstormEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ParticleLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.FPSLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.terrainHeight = new System.Windows.Forms.NumericUpDown();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.color0 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.color2 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.color3 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.color1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.terrainHeight)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(812, 516);
            this.splitContainer1.SplitterDistance = 583;
            this.splitContainer1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(579, 512);
            this.panel1.TabIndex = 0;
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            this.panel1.Leave += new System.EventHandler(this.panel1_Leave);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ParticleLabel,
            this.FPSLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 490);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(579, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ParticleLabel
            // 
            this.ParticleLabel.Name = "ParticleLabel";
            this.ParticleLabel.Size = new System.Drawing.Size(54, 17);
            this.ParticleLabel.Text = "Particles:";
            // 
            // FPSLabel
            // 
            this.FPSLabel.Name = "FPSLabel";
            this.FPSLabel.Size = new System.Drawing.Size(29, 17);
            this.FPSLabel.Text = "FPS:";
            this.FPSLabel.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(221, 512);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(213, 486);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Terrain";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.terrainHeight);
            this.groupBox1.Location = new System.Drawing.Point(7, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 49);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Terain Height";
            // 
            // terrainHeight
            // 
            this.terrainHeight.Location = new System.Drawing.Point(6, 19);
            this.terrainHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.terrainHeight.Name = "terrainHeight";
            this.terrainHeight.Size = new System.Drawing.Size(188, 20);
            this.terrainHeight.TabIndex = 0;
            this.terrainHeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.terrainHeight.ValueChanged += new System.EventHandler(this.terrainHeight_ValueChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(213, 486);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Kinect";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(213, 486);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Paticles";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.shapeContainer1);
            this.groupBox2.Location = new System.Drawing.Point(6, 61);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 64);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Terrain Color";
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(3, 16);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.color1,
            this.color3,
            this.color2,
            this.color0});
            this.shapeContainer1.Size = new System.Drawing.Size(194, 45);
            this.shapeContainer1.TabIndex = 0;
            this.shapeContainer1.TabStop = false;
            // 
            // color0
            // 
            this.color0.BackColor = System.Drawing.Color.White;
            this.color0.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(167)))));
            this.color0.FillGradientColor = System.Drawing.Color.Black;
            this.color0.FillStyle = Microsoft.VisualBasic.PowerPacks.FillStyle.Solid;
            this.color0.Location = new System.Drawing.Point(8, 5);
            this.color0.Name = "color0";
            this.color0.Size = new System.Drawing.Size(30, 30);
            this.color0.Click += new System.EventHandler(this.lowColor_Click);
            // 
            // color2
            // 
            this.color2.BackColor = System.Drawing.Color.White;
            this.color2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(217)))), ((int)(((byte)(87)))));
            this.color2.FillStyle = Microsoft.VisualBasic.PowerPacks.FillStyle.Solid;
            this.color2.Location = new System.Drawing.Point(105, 5);
            this.color2.Name = "color2";
            this.color2.Size = new System.Drawing.Size(30, 30);
            this.color2.Click += new System.EventHandler(this.color2_Click);
            // 
            // color3
            // 
            this.color3.BackColor = System.Drawing.Color.White;
            this.color3.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(43)))), ((int)(((byte)(0)))));
            this.color3.FillStyle = Microsoft.VisualBasic.PowerPacks.FillStyle.Solid;
            this.color3.Location = new System.Drawing.Point(155, 5);
            this.color3.Name = "color3";
            this.color3.Size = new System.Drawing.Size(30, 30);
            this.color3.Click += new System.EventHandler(this.color3_Click);
            // 
            // color1
            // 
            this.color1.BackColor = System.Drawing.Color.White;
            this.color1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(133)))), ((int)(((byte)(8)))));
            this.color1.FillStyle = Microsoft.VisualBasic.PowerPacks.FillStyle.Solid;
            this.color1.Location = new System.Drawing.Point(57, 5);
            this.color1.Name = "color1";
            this.color1.Size = new System.Drawing.Size(30, 30);
            this.color1.Click += new System.EventHandler(this.color1_Click);
            // 
            // SandstormEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 516);
            this.Controls.Add(this.splitContainer1);
            this.Name = "SandstormEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sandstorm Editor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.terrainHeight)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        public System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel FPSLabel;
        private System.Windows.Forms.ToolStripStatusLabel ParticleLabel;
        private System.Windows.Forms.NumericUpDown terrainHeight;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape color3;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape color2;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape color0;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape color1;
    }
}