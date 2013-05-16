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
        private Vector3 _pos;
        private Vector3 _force;

        public Vector3 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Vector3 Force
        {
            get { return _force; }
            set { _force = value; }
        }

        public Particle()
        {
        }        

        public static Particle getParticle(Vector3 pPos, Vector3 pForce)
        {
            Particle p = SharedList.FreeParticles.Get();
            p._pos = pPos;
            p._force = pForce;
            return p;
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
    }
}
