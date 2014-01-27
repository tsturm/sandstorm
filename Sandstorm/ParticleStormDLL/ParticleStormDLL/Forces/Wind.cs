using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ParticleStormDLL.Forces
{
    public class Wind : BaseForce
    {
        public Wind(Vector3 force)
        {
            Force = force;
        }

        public override Vector3 Update(GameTime gameTime)
        {
            return Force;
        }
    }
}
