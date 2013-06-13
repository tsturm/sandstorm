using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Threading.Tasks;


namespace Sandstorm.ParticleSystem.structs
{
    class LinearEmiter : Emiter
    {
        public LinearEmiter(Vector3 pos, Vector3 force, SharedList sharedlist) : base(pos,force,sharedlist)
        {
        }

        override
        public void emit()
        {
            Parallel.For(0, 2, i =>
            {
                Particle p = Particle.getParticle(this._pos + new Vector3(-80 + getRandomFloat(0, 360), 0, -100), new Vector3(0, 0, 0.1f));
                _sharedlist.addParticle(p);
            });
        }
    }
}
