using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sandstorm.ParticleSystem.structs
{
    class FPSCounter
    {
        int _total_frames = 0;
        float _elapsed_time = 0.0f;
        int _fps = 0;

        public void Update(GameTime pGameTime)
        {
            // Update
            _elapsed_time += (float)pGameTime.ElapsedGameTime.TotalMilliseconds;

            // 1 Second has passed
            if (_elapsed_time > 1000.0f)
            {
                _fps = _total_frames;
                _total_frames = 0;
                _elapsed_time = 0;
            }
            _total_frames++;
        }
        public int getFrames()
        {
            return _fps;
        }
    }
}
