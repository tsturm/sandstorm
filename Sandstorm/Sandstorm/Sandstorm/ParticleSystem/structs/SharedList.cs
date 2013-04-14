using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandstorm.ParticleSystem
{
    class SharedList
    {
        private List<Particle> _particles = new List<Particle>();

        public List<Particle> getParticles()
        {
            return this._particles;
        }
    }
}
