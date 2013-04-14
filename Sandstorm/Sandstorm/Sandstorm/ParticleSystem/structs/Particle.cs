using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandstorm.ParticleSystem
{
    class Particle
    {
        Vector3 _pos;
        Vector3 _force;

        public Particle(Vector3 pos, Vector3 force)
        {
            this._pos = pos;
            this._force = force;
        }
        public override string ToString()
        {
            return "Pos: " + _pos + " force: " + _force;
        }
    }
}
