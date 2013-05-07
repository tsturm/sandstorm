using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sandstorm.ParticleSystem.structs;

namespace Sandstorm.ParticleSystem
{
    class Emiter
    {
        Vector3 _pos;
        Vector3 _force;
        SharedList _sharedlist;
        private Random _rand = new Random();
        
        private static float MIN_RAND = 0.01f;
        private static float MAX_RAND = 0.5f;
        public Emiter(Vector3 pos, Vector3 force, SharedList sharedlist)
        {
            this._pos = pos;
            this._force = force;
            this._sharedlist = sharedlist;
        }

        private float getRandomFloat(float min, float max)
        {
            return (float)(min + (_rand.NextDouble() * (max - min)));
        }
        private Vector3 getSmallRandomForce()
        {
            return new Vector3(this.getRandomFloat(MIN_RAND, MAX_RAND), this.getRandomFloat(MIN_RAND, MAX_RAND), this.getRandomFloat(MIN_RAND, MAX_RAND));
        }
        public void emit()
        {
            Particle p = new Particle(this._pos, this._force);

            p.applyExternalForce(this.getSmallRandomForce());

            for(int i=0; i < 100;i++)
                this._sharedlist.addParticle(p);
        }
    }
}
