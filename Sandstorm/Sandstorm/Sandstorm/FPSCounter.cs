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
using System.Globalization;


namespace Sandstorm
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FPSCounter : Microsoft.Xna.Framework.DrawableGameComponent
    {
        /// <summary>
        /// Elapsed time since last frame rate update.
        /// </summary>
        private TimeSpan Elapsed;

        /// <summary>
        /// The actual frame rate.
        /// </summary>
        private int FrameRate;

        /// <summary>
        /// Frames since last frame rate update.
        /// </summary>
        private int Frames;

        /// <summary>
        /// SpriteBatch to draw the text
        /// </summary>
        private SpriteBatch SpriteBatch;

        /// <summary>
        /// SprinteFont to hold the font type
        /// </summary>
        private SpriteFont SpriteFont;

        /// <summary>
        /// Creates an instance of FPSCounter.
        /// </summary>
        public FPSCounter(Game game)
            : base(game)
        {
            //Add ParticleSystem to the game components
            Game.Components.Add(this);

            Frames = 0;
            FrameRate = 0;
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Called when graphics resources need to be loaded.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteFont = Game.Content.Load<SpriteFont>("font\\Font");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            Elapsed += gameTime.ElapsedGameTime;

            if (Elapsed > TimeSpan.FromSeconds(1))
            {
                Elapsed -= TimeSpan.FromSeconds(1);
                FrameRate = Frames;
                Frames = 0;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Called when the DrawableGameComponent needs to be drawn.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            try
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.BlendState = BlendState.Opaque;

                string text = string.Format(CultureInfo.CurrentCulture, "FPS: {0}\n", FrameRate);

                SpriteBatch.Begin();

                SpriteBatch.DrawString(SpriteFont, text, new Vector2(10, 5), Color.White);

                SpriteBatch.End();

                Frames++;
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine("ObjectDisposed FPS-Counter!" + e);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("InvalidOperationException FPS-Counter!" + e);
            }
            base.Draw(gameTime);
        }
    }
}
