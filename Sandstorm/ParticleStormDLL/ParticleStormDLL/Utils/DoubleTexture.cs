using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;

namespace ParticleStormDLL.Utils
{
    public class DoubleTexture
    {
        public Texture2D TextureA { get; set; }
        public Texture2D TextureB { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public bool MipMap { get; set; }
        public SurfaceFormat PreferredFormat { get; set; }
        public bool DoSwap { get; set; }

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
        public DoubleTexture(GraphicsDevice graphicsDevice, int width, int height, bool mipMap, SurfaceFormat preferredFormat)
        {
            Width = width;
            Height = height;
            MipMap = mipMap;
            PreferredFormat = preferredFormat;
            DoSwap = false;

            TextureA = new Texture2D(graphicsDevice, Width, Height, MipMap, PreferredFormat);
            TextureB = new Texture2D(graphicsDevice, Width, Height, MipMap, PreferredFormat);

            BackupA = new Vector4[width * height];
            BackupB = new Vector4[width * height];
        }

        /// <summary>
        /// Swap the two render targets
        /// </summary>
        public void Swap()
        {
            if (DoSwap)
            {
                Texture2D tmp = TextureA;
                TextureA = TextureB;
                TextureB = tmp;
                DoSwap = false;
            }
        }

        /// <summary>
        /// Backup render targets data before device reset
        /// </summary>
        public void Backup()
        {
            TextureA.GetData(BackupA);
            TextureB.GetData(BackupB);
        }

        /// <summary>
        /// Restore render targets data after device reset;
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public void Restore(GraphicsDevice graphicsDevice)
        {
            TextureA.Dispose();
            TextureA = new Texture2D(graphicsDevice, Width, Height, MipMap, PreferredFormat);
            TextureA.SetData(BackupA);

            TextureB.Dispose();
            TextureB = new Texture2D(graphicsDevice, Width, Height, MipMap, PreferredFormat);
            TextureB.SetData(BackupB);
        }
    }
}
