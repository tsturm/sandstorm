using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sandstorm.ParticleSystem.structs.Forces
{
    class Gravity : Force
    {
        public Gravity() : base(new Vector3(0f, -0.1f, 0f))
        {
        }
    }
}
