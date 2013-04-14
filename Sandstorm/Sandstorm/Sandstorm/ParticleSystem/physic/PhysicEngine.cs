using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sandstorm.ParticleSystem.structs;

namespace Sandstorm.ParticleSystem.physic
{
    class PhysicEngine
    {
        private SharedList _sharedList = null;
        private FPSCounter _fpsCounter = new FPSCounter();

        public PhysicEngine(SharedList pList)
        {
            this._sharedList = pList;

            _sharedList.getParticles().Add(new Particle(new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f))); //Test-Particle
        }

        public void Update(GameTime pGameTime) //Update physic
        {
            _fpsCounter.Update(pGameTime);
        }

        public void Draw() //Nothing to draw.. normally
        {
            
        }

        public int getFPS()
        {
            return _fpsCounter.getFrames();
        }
    }
}
