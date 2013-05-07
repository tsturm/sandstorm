using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Sandstorm.ParticleSystem.structs;

using System.Threading;
using System.Threading.Tasks;

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
        
        private Instancing _instancing = null;

        public DrawEngine(GraphicsDevice pGraphicsDevice, ContentManager pContentManager, Camera pCamera,SharedList pList)
        {
            this._camera = pCamera;
            this._graphicsDevice = pGraphicsDevice;
            this._contentManager = pContentManager;
            this._sharedList = pList;

            _spriteBatch = new SpriteBatch(this._graphicsDevice);
            _font = _contentManager.Load<SpriteFont>("font/FPSFont");
            _instancing = new Instancing(_graphicsDevice,_contentManager,_camera,_sharedList);
        }


        public void Update(GameTime pGameTime) //Update DrawEngine
        {
        }

        public int getFPS()
        {
            return _fpsCounter.getFrames();
        }

        
        public void Draw(int pFPSDraw,int pFPSPhysic) //Draw all Particles
        {
            _fpsCounter.Update();

            _spriteBatch.Begin();
            String s = string.Format("Particles={0}, DrawEngineFPS={1}, PhysicEngineFPS={2} Grafik={3}", _sharedList.getParticles().Count, pFPSDraw, pFPSPhysic,_graphicsDevice.Adapter.Description);
        
            Vector2 screenSize = new Vector2(_graphicsDevice.Viewport.Height, _graphicsDevice.Viewport.Width);
            Vector2 pos = _font.MeasureString(s);
            
            _spriteBatch.DrawString(_font, s, new Vector2(0,0), Color.White);
            _spriteBatch.End();


            //Draw Particles
            RasterizerState prevRasterizerState = _graphicsDevice.RasterizerState;
            BlendState prevBlendState = _graphicsDevice.BlendState;
            
            _instancing.Draw();

            _graphicsDevice.BlendState = prevBlendState;
            _graphicsDevice.RasterizerState = prevRasterizerState;
        }
    }
}
