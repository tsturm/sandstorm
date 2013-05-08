using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandstorm.ParticleSystem.structs;

namespace Sandstorm.ParticleSystem
{
    public class SharedList
    {
        public List<Particle> _particles = new List<Particle>();

        private int _maxCount = 10000;

        public List<Particle> getParticles()
        {
            return this._particles;
        }

        private readonly object syncLock = new object();
        public void addParticle(Particle pParticle)
        {
            lock (syncLock) {
                this._particles.Add(pParticle);
                if (this._particles.Count > _maxCount)
                {
                    this._particles.RemoveAt(0);
                }
            }
        }
    }
}
