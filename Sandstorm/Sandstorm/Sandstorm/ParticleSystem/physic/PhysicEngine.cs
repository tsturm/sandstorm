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

namespace Sandstorm.ParticleSystem.physic
{
    class PhysicEngine
    {
        private SharedList _sharedList = null;
        private FPSCounter _fpsCounter = new FPSCounter();

        private List<Vector3> _forces = new List<Vector3>();

        private HeightMap _heightMap;

        public PhysicEngine(SharedList pList, HeightMap heightMap)
        {
            this._sharedList = pList;
            this._heightMap = heightMap;

            this._forces.Add(new Vector3(0f, -0.1f, 0f));
        }

        public void Update(GameTime pGameTime) //Update physic
        {
            Parallel.ForEach(_sharedList.getParticles(), p =>
            {
                p.move();//move the Particle


                foreach (Vector3 f in _forces)//apply external forces (Gravitation etc)
                {
                    p.applyExternalForce(f);
                }
                //check colision
                if ( _heightMap != null)
                    if (p.getPosition().Y < _heightMap.getHeight(p.getPosition().X, p.getPosition().Z))
                    {
                        Vector3 v = new Vector3(p.getPosition().X, _heightMap.getHeight(p.getPosition().X, p.getPosition().Z), p.getPosition().Z);
                        Vector3 v1 = v - new Vector3(p.getPosition().X + 0.01f, _heightMap.getHeight(p.getPosition().X + 0.01f, p.getPosition().Z), p.getPosition().Z);
                        Vector3 v2 = v - new Vector3(p.getPosition().X, _heightMap.getHeight(p.getPosition().X, p.getPosition().Z + 0.01f), p.getPosition().Z + 0.01f);
                        Vector3 normal = Vector3.Cross(v1,v2);
                        normal.Normalize();
                        p.setForce((p.getForce() - ((2 * Vector3.Dot(p.getForce(), normal)) * normal)) * 0.6f);//*0.6f Particle lose 40% of ist Power on colision
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
