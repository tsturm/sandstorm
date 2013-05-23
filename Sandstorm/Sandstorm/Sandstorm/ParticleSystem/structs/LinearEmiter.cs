using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
            for (int i = 0; i < 100; i++)
            {
                Particle p = new Particle(this._pos+new Vector3(60+getRandomFloat(0,180),10,-40), new Vector3(0,0,0.1f));
                _sharedlist.addParticle(p);
            }
        }
    }
}
