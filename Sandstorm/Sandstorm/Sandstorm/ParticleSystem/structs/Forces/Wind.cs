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
            float h = p.Pos.Y/(_heightMap.getHeightData(p.Pos.X, p.Pos.Z) + 100);
            Vector3 f = Vector3.Cross(_heightMap.getNormal(p.Pos.X, p.Pos.Z), new Vector3(force.Z, force.Y, force.X));
            p.applyExternalForce(f /**(1-h)*/);
            p.applyTemporalExternalForce(f);
            p.applyExternalForce(force*-1);
        }
    }
}
