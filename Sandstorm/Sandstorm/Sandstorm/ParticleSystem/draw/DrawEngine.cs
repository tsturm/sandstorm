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
        private GraphicsDevice _graphicsDevice = null;
        private ContentManager _contentManager = null;
        private SharedList _sharedList = null;

        private FPSCounter _fpsCounter = new FPSCounter();        
        private SpriteBatch _spriteBatch = null;
        private SpriteFont _font = null;
        
        private Instancing _instancing = null;

        public DrawEngine(GraphicsDevice pGraphicsDevice, ContentManager pContentManager, SharedList pList)
        {
            this._graphicsDevice = pGraphicsDevice;
            this._contentManager = pContentManager;
            this._sharedList = pList;

            _spriteBatch = new SpriteBatch(this._graphicsDevice);
            _font = _contentManager.Load<SpriteFont>("font/FPSFont");
            _instancing = new Instancing(_graphicsDevice,_contentManager,_sharedList);
        }


        public void Update(GameTime pGameTime) //Update DrawEngine
        {
        }

        public int getFPS()
        {
            return _fpsCounter.getFrames();
        }


        public RenderTarget2D Draw(Camera pCamera, RenderTarget2D pTarget, int pFPSDraw, int pFPSPhysic) //Draw all Particles
        {
            _fpsCounter.Update();
            
            //Draw Particles
            RasterizerState prevRasterizerState = _graphicsDevice.RasterizerState;
            BlendState prevBlendState = _graphicsDevice.BlendState;

           RenderTarget2D ret = _instancing.Draw(pCamera,pTarget);

            _graphicsDevice.BlendState = prevBlendState;
            _graphicsDevice.RasterizerState = prevRasterizerState;

            return ret;
        }
    }
}
