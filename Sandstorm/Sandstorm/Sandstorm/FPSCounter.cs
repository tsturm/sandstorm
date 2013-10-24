using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Sandstorm
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FPSCounter : Microsoft.Xna.Framework.GameComponent
    {
        #region FIELDS

        private float _Elapsed;
        private float _FrameRate;
        private float _Frames;

        #endregion

        #region PROPPERTIES

        public float FPS
        {
            get { return _FrameRate; }
            set {}
        }

        #endregion

        public FPSCounter(Game game) : base(game)
        {
            _Frames = 0;
            _Elapsed = 0;
            _FrameRate = 0;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            _Elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_Elapsed > 1.0f)
            {
                _Elapsed -= 1.0f;
                _FrameRate = _Frames;
                _Frames = 0;
            }
            else
            {
                _Frames += 1;
            }

            base.Update(gameTime);
        }
    }
}
