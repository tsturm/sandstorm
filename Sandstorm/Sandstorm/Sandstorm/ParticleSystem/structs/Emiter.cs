using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sandstorm.ParticleSystem.structs;

using System.Threading;
using System.Threading.Tasks;

namespace Sandstorm.ParticleSystem
{
    class Emiter
    {
        protected Vector3 _pos;
        protected Vector3 _force;
        protected SharedList _sharedlist;
        protected Random _rand = new Random();

        protected static float MIN_RAND = -0.5f;
        protected static float MAX_RAND = 0.5f;
        public Emiter(Vector3 pos, Vector3 force, SharedList sharedlist)
        {
            this._pos = pos;
            this._force = force;
            this._sharedlist = sharedlist;
        }

        protected float getRandomFloat(float min, float max)
        {
            return (float)((_rand.NextDouble() * (Math.Abs(max) + Math.Abs(min)) - Math.Abs(min)));
        }

        private Vector3 getSmallRandomForce()
        {
            return new Vector3(this.getRandomFloat(MIN_RAND, MAX_RAND), this.getRandomFloat(MIN_RAND, MAX_RAND), this.getRandomFloat(MIN_RAND, MAX_RAND));
        }

        virtual
        public void emit()
        {
            Parallel.For(0, 1, i =>
            {

                Particle p = new Particle(this._pos, this._force);
                p.applyExternalForce(this.getSmallRandomForce());
                this._sharedlist.addParticle(p);
            });
        }
    }
}
