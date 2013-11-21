using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;

namespace ParticleStormDLL.Utils
{
    public class DoubleTarget
    {
        public RenderTarget2D TargetA { get; set; }
        public RenderTarget2D TargetB { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public bool MipMap { get; set; }
        public SurfaceFormat PreferredFormat { get; set; }
        public DepthFormat PreferredDepthFormat { get; set; }

        public Vector4[] BackupA;
        public Vector4[] BackupB;

        /// <summary>
        /// Creates an instance of DoubleTarget
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mipMap"></param>
        /// <param name="preferredFormat"></param>
        /// <param name="preferredDepthFormat"></param>
        public DoubleTarget(GraphicsDevice graphicsDevice, int width, int height, bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat)
        {
            Width = width;
            Height = height;
            MipMap = mipMap;
            PreferredFormat = preferredFormat;
            PreferredDepthFormat = preferredDepthFormat;

            TargetA = new RenderTarget2D(graphicsDevice, Width, Height, MipMap, PreferredFormat, PreferredDepthFormat);
            TargetB = new RenderTarget2D(graphicsDevice, Width, Height, MipMap, PreferredFormat, PreferredDepthFormat);

            BackupA = new Vector4[width * height];
            BackupB = new Vector4[width * height];
        }

        /// <summary>
        /// Swap the two render targets
        /// </summary>
        public void Swap()
        {
            RenderTarget2D tmp = TargetA;
            TargetA = TargetB;
            TargetB = tmp;
        }

        /// <summary>
        /// Backup render targets data before device reset
        /// </summary>
        public void Backup()
        {
            TargetA.GetData(BackupA);
            TargetB.GetData(BackupB);
        }

        /// <summary>
        /// Restore render targets data after device reset;
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public void Restore(GraphicsDevice graphicsDevice)
        {
            TargetA.Dispose();
            TargetA = new RenderTarget2D(graphicsDevice, Width, Height, MipMap, PreferredFormat, PreferredDepthFormat);
            TargetA.SetData(BackupA);

            TargetB.Dispose();
            TargetB = new RenderTarget2D(graphicsDevice, Width, Height, MipMap, PreferredFormat, PreferredDepthFormat);
            TargetB.SetData(BackupB);
        }
    }
}
