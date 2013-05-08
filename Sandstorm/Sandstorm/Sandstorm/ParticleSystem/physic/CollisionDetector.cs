using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandstorm.ParticleSystem.structs;
using Sandstorm.Terrain;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Sandstorm.ParticleSystem.physic
{
    
    class CollisionDetector
    {
        private SharedList _particleList = null;
        private HeightMap _heightMap = null;

        private List<Particle>[, ,] _cells;
        private static int _cellsize = 10;
        private static int _cellsPerDimension;

        public CollisionDetector(SharedList particleList, HeightMap heightMap)
        {
            _particleList = particleList;
            _heightMap = heightMap;
            initCells();
        }

        public void initCells()
        {
            _cellsPerDimension = _heightMap.getWidth()/_cellsize;
            _cells = new List<Particle>[_cellsPerDimension, _cellsPerDimension/2, _cellsPerDimension];
            for (int x = 0; x < _cellsPerDimension; x++)
                for (int y = 0; y < _cellsPerDimension/2; y++)
                    for (int z = 0; z < _cellsPerDimension; z++)
                        _cells[x, y, z] = new List<Particle>();
        }

        public List<Particle> getCell(float x, float y, float z)
        {
            x += _heightMap.getWidth() / 2;
            y += _heightMap.getWidth() / 2;
            z += _heightMap.getWidth() / 2;
            x /= _cellsize;
            y /= _cellsize;
            z /= _cellsize;
            int cx = (int)x;
            int cy = (int)y;
            int cz = (int)z;
            if(cx>=0 && cy>=0 && cz>=0)
                if (cx < _cellsPerDimension && cy < _cellsPerDimension/2 && cz < _cellsPerDimension)
                    return _cells[cx, cy, cz];
            return new List<Particle>();
        }

        public List<Particle> getCell(Vector3 particlePos)
        {
            return getCell(particlePos.X, particlePos.Y, particlePos.Z);
        }

        public void checkCollisions()
        {
            foreach(Particle p in _particleList.getParticles())
            {
                List<Particle> oldCell = getCell(p.getOldPosition());
                List<Particle> newCell = getCell(p.getPosition());
                if (oldCell != newCell)
                {
                    if (oldCell.Contains(p))
                    {
                        oldCell.Remove(p);
                        newCell.Add(p);
                    }
                    else
                    {
                        newCell.Add(p);
                    }
                }
                else
                {
                    if (!oldCell.Contains(p))
                        newCell.Add(p);
                }
            }
            Parallel.ForEach(_particleList.getParticles(), p =>
            {
                p.collide(getCell(p.getPosition()).ToArray());
                p.collide(getCell(p.getPosition() + new Vector3(0, 0, 1)).ToArray());
                p.collide(getCell(p.getPosition() + new Vector3(0, 0, -1)).ToArray());
                p.collide(getCell(p.getPosition() + new Vector3(0, 1, 0)).ToArray());
                p.collide(getCell(p.getPosition() + new Vector3(0, -1, 0)).ToArray());
                p.collide(getCell(p.getPosition() + new Vector3(1, 0, 0)).ToArray());
                p.collide(getCell(p.getPosition() + new Vector3(-1, 0, 0)).ToArray());
            });
        }
    }
}
