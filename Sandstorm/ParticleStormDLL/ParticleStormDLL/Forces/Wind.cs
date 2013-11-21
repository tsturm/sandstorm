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

        public Wind(Vector3 direction, float speed)
        {
            Direction = direction;
            Speed = speed;
        }

        public override Vector3 Update(GameTime gameTime)
        {
            Direction.Normalize();
            Force = Direction * Speed;
            return Force;
        }
    }
}
