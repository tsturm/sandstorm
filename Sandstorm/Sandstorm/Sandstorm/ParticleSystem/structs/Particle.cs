using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sandstorm.ParticleSystem.structs;

namespace Sandstorm.ParticleSystem
{
    public class Particle
    {        
        Vector3 _pos;
        Vector3 _force;
        public Particle(Vector3 pPos, Vector3 pForce)
        {
            _pos = pPos;
            _force = pForce;
        }

        public void move()
        {
            this._pos += this._force;
        }

        public void applyExternalForce(Vector3 pForce)
        {
            this._force += pForce;
        }

        override public string ToString()
        {
            return "Pos: " + this._pos + " force: " + this._force;
        }

        public Vector3 getPosition()
        {
            return this._pos;
        }

        public Vector3 getForce()
        {
            return this._force;
        }

        public void setForce(Vector3 pForce)
        {
            this._force = pForce;
        }
    }
}
