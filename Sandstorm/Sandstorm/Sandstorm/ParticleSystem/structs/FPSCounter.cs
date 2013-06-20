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
        int _lastupdate = 0;
        int _fps = 0;

        public void Update()
        {
            int time = System.Environment.TickCount;
            // Update

            // 1 Second has passed
            if (time-_lastupdate > 1000)
            {
                _fps = _total_frames;
                _total_frames = 0;
                _lastupdate = time;
            }
            _total_frames++;
        }
        public int getFrames()
        {
            return _fps;
        }
    }
}
