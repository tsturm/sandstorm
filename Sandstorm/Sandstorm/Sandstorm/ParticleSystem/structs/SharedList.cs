using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandstorm.ParticleSystem.structs;

namespace Sandstorm.ParticleSystem
{
    public class SharedList
    {
        public static int _maxCount = 1;

        private static readonly ObjectPool<Particle> _freeParticles = new ObjectPool<Particle>(_maxCount);
        private Particle[] _particles = new Particle[_maxCount];

        private int _pos = 0;
        private int _count = 0;

        public static ObjectPool<Particle> FreeParticles
        {
            get { return _freeParticles; }
        }
        public Particle[] Particles
        {
            get { return _particles; }
        }

        public int Count
        {
            get { return _count; }
        }

        public Particle[] getParticles()
        {
            return this._particles;
        }

        private readonly object syncLock = new object();
        public void addParticle(Particle pParticle)
        {
            lock (syncLock) {
                if (_count >= _maxCount)
                {
                    Particle p = this._particles[_pos];
                    _freeParticles.Put(p);
                    _count--;
                }

                this._particles[_pos] = pParticle;
                _count++;
                _pos = (++_pos) % _maxCount;

               
            }
        }
    }
}
