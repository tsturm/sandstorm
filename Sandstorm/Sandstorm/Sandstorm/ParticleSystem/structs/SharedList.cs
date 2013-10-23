using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandstorm.ParticleSystem.structs;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sandstorm.ParticleSystem
{
    public class SharedList
    {
        private GraphicsDevice _graphicsDevice = null;
        public static int _maxCount = 1;

        private static readonly ObjectPool<Particle> _freeParticles = new ObjectPool<Particle>(_maxCount);
        private Particle[] _particles = new Particle[_maxCount];

        private int _pos = 0;
        private int _count = 0;

        private Texture2D _particlePositions = null;
        private static int _squareSize = 512;

        private static SharedList instance = null;
       
        public static SharedList getInstance(GraphicsDevice pGraphicsDevice) 
        {
            if (instance == null)
                instance = new SharedList(pGraphicsDevice);
            return instance;
        }
        private SharedList() {} 
        private SharedList(GraphicsDevice pGraphicsDevice)
        {
            _graphicsDevice = pGraphicsDevice;
            _particlePositions = new Texture2D(_graphicsDevice, SharedList.SquareSize, SharedList.SquareSize, false, SurfaceFormat.Vector4);

            Vector4[] myVector = new Vector4[SharedList.SquareSize * SharedList.SquareSize];
            for (int x = 0; x < _squareSize; x++)
                for (int y = 0; y < _squareSize; y++)
                {
                    myVector[x * _squareSize + y].X = x * 10f;
                    myVector[x * _squareSize + y].Z = y * 10f;
                }
            _particlePositions.SetData(myVector);
        }

        public Texture2D ParticlePositions
        {
            get { return _particlePositions; }
            set { _particlePositions = value; }
        }

        public static int SquareSize
        {
            get { return _squareSize; }
        }

        public static ObjectPool<Particle> FreeParticles
        {
            get { return _freeParticles; }
        }
        public Particle[] Particles
        {
            get { return _particles; }
        }

        public int Count
        {
            get { return _count; }
        }

        public Particle[] getParticles()
        {
            return this._particles;
        }

        private readonly object syncLock = new object();
        public void addParticle(Particle pParticle)
        {
            lock (syncLock) {
                if (_count >= _maxCount)
                {
                    Particle p = this._particles[_pos];
                    _freeParticles.Put(p);
                    _count--;
                }

                this._particles[_pos] = pParticle;
                _count++;
                _pos = (++_pos) % _maxCount;

               
            }
        }


    }
}
