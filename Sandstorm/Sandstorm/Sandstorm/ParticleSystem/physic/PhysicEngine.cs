using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Sandstorm.ParticleSystem.structs;
using Sandstorm.Terrain;
using Sandstorm.ParticleSystem.structs.Forces;

namespace Sandstorm.ParticleSystem.physic
{
    class PhysicEngine
    {
        private SharedList _sharedList = null;
        private FPSCounter _fpsCounter = new FPSCounter();

        private List<Force> _forces = new List<Force>();

        private HeightMap _heightMap;

        private CollisionDetector _collisionDetector;

        public PhysicEngine(SharedList pList, HeightMap heightMap)
        {
            this._sharedList = pList;
            this._heightMap = heightMap;
            _collisionDetector = new CollisionDetector(pList, _heightMap);

            this._forces.Add(new Gravity());
            //this._forces.Add(new Force(new Vector3(-0.00f, -0.0f, 0.1f)));
            this._forces.Add(new Wind(new Vector3(-0.0f, -0.0f, -0.1f), heightMap));
        }

        public void Update(GameTime pGameTime) //Update physic
        {
            moveParticles();
            _collisionDetector.checkCollisions();
            applyForces();
            
        }

        private void moveParticles()
        {
            Parallel.ForEach(_sharedList.getParticles(), p =>
            {
                if (p != null)
                {
                    p.move();
                }
            });
        }

        private void applyForces()
        {
            Parallel.ForEach(_sharedList.getParticles(), p =>
            {
                if (p != null)
                {
                    foreach (Force f in _forces)//apply external forces (Gravitation etc)
                    {
                        f.apply_to(p);
                    }
                }
            });
        }

        public void Draw() //Nothing to draw.. normally
        {
            _fpsCounter.Update();
        }

        public int getFPS()
        {
            return _fpsCounter.getFrames();
        }
    }
}
