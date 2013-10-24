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
        
        public void Draw(Camera pCamera) //Draw all Particles
        {            
            //Draw Particles
            RasterizerState prevRasterizerState = _graphicsDevice.RasterizerState;
            BlendState prevBlendState = _graphicsDevice.BlendState;

            _instancing.Draw(pCamera);

            _graphicsDevice.BlendState = prevBlendState;
            _graphicsDevice.RasterizerState = prevRasterizerState;            
        }
    }
}
