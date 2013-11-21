using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ParticleStormDLL.Utils
{
    public class Randomizer
    {
        private Random Random;

        public Randomizer()
        {
            Random = new Random();
        }

        public float nextFloat(float min, float max)
        {
            return (float)Random.NextDouble() * (max - min) + min;
        }

        public Vector2 nextVector2(Vector2 min, Vector2 max)
        {
            Vector2 randomized = new Vector2();

            randomized.X = (float)Random.NextDouble() * (max.X - min.X) + min.X;
            randomized.Y = (float)Random.NextDouble() * (max.Y - min.Y) + min.Y;

            return randomized;
        }

        public Vector3 nextVector3(Vector3 min, Vector3 max)
        {
            Vector3 randomized = new Vector3();

            randomized.X = (float)Random.NextDouble() * (max.X - min.X) + min.X;
            randomized.Y = (float)Random.NextDouble() * (max.Y - min.Y) + min.Y;
            randomized.Z = (float)Random.NextDouble() * (max.Z - min.Z) + min.Z;

            return randomized;
        }

        public Vector4 nextVector4(Vector4 min, Vector4 max)
        {
            Vector4 randomized = new Vector4();

            randomized.X = (float)Random.NextDouble() * (max.X - min.X) + min.X;
            randomized.Y = (float)Random.NextDouble() * (max.Y - min.Y) + min.Y;
            randomized.Z = (float)Random.NextDouble() * (max.Z - min.Z) + min.Z;
            randomized.W = (float)Random.NextDouble() * (max.W - min.W) + min.W;

            return randomized;
        }

        public Vector4 nextVector4(Vector3 min, Vector3 max)
        {
            Vector4 randomized = new Vector4();

            randomized.X = (float)Random.NextDouble() * (max.X - min.X) + min.X;
            randomized.Y = (float)Random.NextDouble() * (max.Y - min.Y) + min.Y;
            randomized.Z = (float)Random.NextDouble() * (max.Z - min.Z) + min.Z;
            randomized.W = 1.0f;

            return randomized;
        }

        public Vector4 nextVector4(Color min, Color max)
        {
            Vector4 randomized = new Vector4();

            randomized.X = (float)Random.Next(min.R, max.R) / 255.0f;
            randomized.Y = (float)Random.Next(min.G, max.G) / 255.0f;
            randomized.Z = (float)Random.Next(min.B, max.B) / 255.0f;
            randomized.W = (float)Random.Next(min.A, max.A) / 255.0f;

            return randomized;
        }

        public Color nextColor(Color min, Color max)
        {
            Color randomized = new Color();

            randomized.R = (byte)Random.Next(min.R, max.R);
            randomized.G = (byte)Random.Next(min.G, max.G);
            randomized.B = (byte)Random.Next(min.B, max.B);
            randomized.A = (byte)Random.Next(min.A, max.A);

            return randomized;
        }
    }
}
