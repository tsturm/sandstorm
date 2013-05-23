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
        private List<Particle> _ignoreCell;
        private static int _cellsize = 2;
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
            _cells = new List<Particle>[_cellsPerDimension, _cellsPerDimension, _cellsPerDimension];
            for (int x = 0; x < _cellsPerDimension; x++)
                for (int y = 0; y < _cellsPerDimension; y++)
                    for (int z = 0; z < _cellsPerDimension; z++)
                        _cells[x, y, z] = new List<Particle>();
            _ignoreCell = new List<Particle>();
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
                if (cx < _cellsPerDimension && cy < _cellsPerDimension && cz < _cellsPerDimension)
                    return _cells[cx, cy, cz];
            return null;
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
                if (oldCell != null)
                {
                    List<Particle> newCell = getCell(p.getPosition());
                    if (newCell != null)
                    {
                        if (oldCell != newCell)
                        {
                            newCell.Add(p);
                            if (oldCell.Contains(p))
                                oldCell.Remove(p);
                        }
                    }
                }
            }
            Parallel.ForEach(_particleList.getParticles(), p =>
            {
                List<Particle> cell = getCell(p.getPosition());
                if (cell != null)
                {
                    p.collide(cell.ToArray());
                }
            });
        }
    }
}
