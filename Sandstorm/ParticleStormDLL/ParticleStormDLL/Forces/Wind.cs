using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ParticleStormDLL.Forces
{
    public class Wind : BaseForce
    {
        public float Speed { get; set; }
        public Vector3 Direction { get; set; }

        private Random rand = new Random();

        public Wind(Vector3 direction, float speed)
        {
            Direction = direction;
            Speed = speed;
        }

        public override Vector3 Update(GameTime gameTime)
        {
            //Direction += new Vector3((float)(rand.NextDouble()-0.5),(float)(rand.NextDouble()-0.5),(float)(rand.NextDouble()-0.5));
            Direction.Normalize();
            Force = Direction * Speed;
            return Force;
        }
    }
}
