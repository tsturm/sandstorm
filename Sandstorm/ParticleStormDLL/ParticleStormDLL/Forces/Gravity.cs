using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ParticleStormDLL.Forces
{
    public class Gravity : BaseForce
    {
        public Gravity()
        {
            Force = new Vector3(0.0f, -9.81f, 0.0f);
        }

        public override Vector3 Update(GameTime gameTime)
        {
            return Force;
        }
    }
}
