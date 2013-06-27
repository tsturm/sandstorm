using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Sandstorm.ParticleSystem.structs
{
    class Force
    {
        Vector3 _force;
        public Vector3 force
        {
            get { return _force; }
            set { _force = value; }
        }

        public Force(Vector3 force)
        {
            this.force = force;
        }

        public virtual void apply_to(Particle p)
        {
            p.applyExternalForce(force);
        }

        public void apply_to(Particle[] particles)
        {
            Parallel.ForEach(particles, p =>
            {
                if(p!=null)
                    apply_to(p);
            });
        }
    }
}
