using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ParticleStormDLL.Utils
{
    class BitPacker
    {
        public static Vector4 PackBits(Color color1, Color color2) {
            int colorAsInt1 = BitConverter.ToInt32(new byte[] { color1.R, color1.G, color1.B, color1.A }, 0);
            int colorAsInt2 = BitConverter.ToInt32(new byte[] { color2.R, color2.G, color2.B, color2.A }, 0);
            return new Vector4(colorAsInt1, colorAsInt1, colorAsInt2, 0);
        }
    }
}
