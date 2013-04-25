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
        public Vector3 _pos;
        public Vector3 _force;

        public Particle(Vector3 pos, Vector3 force)
        {
            this._pos = pos;
            this._force = force;
        }

        public void move()
        {
            this._pos += this._force;
        }

        public void applyExternalForce(Vector3 force)
        {
            this._force += force;
        }

        public override string ToString()
        {
            return "Pos: " + _pos + " force: " + _force;
        }

        public Vector3 getPosition()
        {
            return _pos;
        }

        public Vector3 getForce()
        {
            return _force;
        }

        public void setForce(Vector3 force)
        {
            _force = force;
        }
    }
}
