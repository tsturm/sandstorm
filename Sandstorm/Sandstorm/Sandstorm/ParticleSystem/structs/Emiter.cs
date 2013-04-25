using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sandstorm.ParticleSystem
{
    class Emiter
    {
        Vector3 _pos;
        Vector3 _force;
        SharedList _sharedlist;

        public Emiter(Vector3 pos, Vector3 force, SharedList sharedlist)
        {
            this._pos = pos;
            this._force = force;
            this._sharedlist = sharedlist;
        }

        public void emit()
        {
            this._sharedlist.addParticle(new Particle(this._pos,this._force));
        }
    }
}
