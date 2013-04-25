using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandstorm.ParticleSystem
{
    class SharedList
    {
        private List<Particle> _particles = new List<Particle>();
        private int _maxCount = 1000;

        public List<Particle> getParticles()
        {
            return this._particles;
        }

        public void addParticle(Particle particle)
        {
            this._particles.Add(particle);
            if (this._particles.Count > _maxCount)
            {
                this._particles.RemoveAt(0);
            }
        }
    }
}
