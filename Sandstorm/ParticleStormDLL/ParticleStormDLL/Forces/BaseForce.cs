using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ParticleStormDLL.Forces
{
    public abstract class BaseForce
    {
        public Vector3 Force { get; set; }

        public abstract Vector3 Update(GameTime gameTime);
    }
}
