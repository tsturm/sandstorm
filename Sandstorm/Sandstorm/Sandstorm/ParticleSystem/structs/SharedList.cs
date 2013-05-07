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

        private int _maxCount = 100000;

        public List<Particle> getParticles()
        {
            return this._particles;
        }

        public void addParticle(Particle pParticle)
        {
            this._particles.Add(pParticle);
            if (this._particles.Count > _maxCount)
            {
                this._particles.RemoveAt(0);
            }
        }
    }
}
