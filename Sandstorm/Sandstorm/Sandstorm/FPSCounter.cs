

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Sandstorm
{
    public class FPSCounter : DrawableGameComponent
    {

        #region FIELDS
        private int _frameRate = 0;
        private int _frameCounter = 0;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        #endregion

        #region PROPPERTIES
        public float FPS
        {
            get { return _frameRate; }
            set { }
        }
        #endregion


        public FPSCounter(Game game)
            : base(game)
        {
        }


        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _frameCounter++;
            base.Draw(gameTime);
        }
    }
}

