using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sandstorm.Terrain;

namespace Sandstorm.ParticleSystem.structs.Forces
{
    class Wind : Force
    {
        protected Random _rand = new Random();
        HeightMap _heightMap;
        public Wind(Vector3 direction, HeightMap heightmap)
            : base(direction)
        {
            _heightMap = heightmap;
        }

        public override void apply_to(Particle p)
        {
            /*Vector3 v1 = p.Pos+force;
            double h1 = _heightMap.getHeightData(p.Pos.X, p.Pos.Y);
            double h2 = _heightMap.getHeightData(v1.X, v1.Y);*/
            //p.applyExternalForce(Vector3.Cross(_heightMap.getNormal(p.Pos.X, p.Pos.Z), force) );
            /*p.applyExternalForce( force);
            p.applyTemporalExternalForce(_heightMap.getNormal(p.Pos.X, p.Pos.Z)*0.1f);*/
            /*float h1 = _heightMap.getHeightData(p.Pos.X, p.Pos.Z);
            float h2 = _heightMap.getHeightData(p.Pos.X + force.X, p.Pos.Z + force.Z);
            Vector3 f = force;
            f.Y = h2 - h1;
            f.Normalize();
            Vector3 rdir = new Vector3((float)_rand.NextDouble() , (float)_rand.NextDouble() , (float)_rand.NextDouble());
            //rdir.Normalize();
            rdir *= 0.01f;
            f = (f * force);// +(rdir * force);
            p.applyExternalForce(f*);*/
            float h = p.Pos.Y/(_heightMap.getHeightData(p.Pos.X, p.Pos.Z) + 100);
            Vector3 f = Vector3.Cross(_heightMap.getNormal(p.Pos.X, p.Pos.Z), new Vector3(force.Z, force.Y, force.X));
            //p.applyExternalForce(f *(1-h));
            //p.applyTemporalExternalForce(f);
            p.applyExternalForce(force*-1);
            Vector3 f2 = new Vector3(0, _heightMap.getHeightData(p.Pos.X, p.Pos.Z) - _heightMap.getHeightData(p.OldPos.X, p.OldPos.Z), 0);
            f2 *= 0.1f;
            p.applyTemporalExternalForce(f2);
        }
    }
}
