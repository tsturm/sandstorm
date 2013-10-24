using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;


namespace Sandstorm
{
    public class FPSCounter
    {
        private static FPSCounter instance = null;
        public static FPSCounter getInstance()
        {
            if (instance == null)
                instance = new FPSCounter();
            return instance;
        }

        private FPSCounter() { }

        Stopwatch _sw = new Stopwatch();
        int _total_frames = 0;
        int _lastupdate = 0;
        int _fps = 0;

        int _fpsCount = 0;
        double currentMilliSec = 0;
        double prevMilliSec = 0;

        public void Update()
        {
            int time = System.Environment.TickCount;
            // Update

            // 1 Second has passed
            if (time - _lastupdate > 1000)
            {
                _fps = _total_frames;
                _total_frames = 0;
                _lastupdate = time;
            }
            _total_frames++;
        }

        public void Measure()
        {
            if (!_sw.IsRunning)
            {
                _sw.Start();
            }
            else
            {
                TimeSpan ts = _sw.Elapsed;
                currentMilliSec = ts.TotalMilliseconds - prevMilliSec;
               
                _fpsCount += 1;
                if (currentMilliSec >= 1000.0f)
                {
                    prevMilliSec = ts.TotalMilliseconds;
                    _fps = _fpsCount;
                    _fpsCount = 0;
                    currentMilliSec = 0;
                }
            }
        }

        public int getFrames()
        {
            return _fps;
        }
    }
}
