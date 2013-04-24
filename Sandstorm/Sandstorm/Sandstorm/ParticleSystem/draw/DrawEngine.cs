using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Sandstorm.ParticleSystem.structs;


namespace Sandstorm.ParticleSystem.draw
{
    class DrawEngine
    {
        private Camera _camera = null;
        private GraphicsDevice _graphicsDevice = null;
        private ContentManager _contentManager = null;
        private SharedList _sharedList = null;

        private FPSCounter _fpsCounter = new FPSCounter();


        private SpriteBatch _spriteBatch = null;
        private SpriteFont _font = null;


        public DrawEngine(GraphicsDevice pGraphicsDevice, ContentManager pContentManager, Camera pCamera,SharedList pList)
        {
            this._camera = pCamera;
            this._graphicsDevice = pGraphicsDevice;
            this._contentManager = pContentManager;
            this._sharedList = pList;

            _spriteBatch = new SpriteBatch(this._graphicsDevice);
            _font = _contentManager.Load<SpriteFont>("font/FPSFont");
        }


        public void Update(GameTime pGameTime) //Update DrawEngine
        {
            _fpsCounter.Update(pGameTime);
        }

        public int getFPS()
        {
            return _fpsCounter.getFrames();
        }

        public void Draw(int pFPSDraw,int pFPSPhysic) //Draw all Particles
        {         
            
            _spriteBatch.Begin();
            String s = string.Format("Particles={0}, DrawEngineFPS={1}, PhysicEngineFPS={2}", _sharedList.getParticles().Count, pFPSDraw, pFPSPhysic);
        
            Vector2 screenSize = new Vector2(_graphicsDevice.Viewport.Height, _graphicsDevice.Viewport.Width);
            Vector2 pos = _font.MeasureString(s);
            
            _spriteBatch.DrawString(_font, s, new Vector2(0,0), Color.White);
            _spriteBatch.End();


            foreach (Particle p in _sharedList.getParticles())
            {
                Console.WriteLine(p);
            }
        }
    }
}
