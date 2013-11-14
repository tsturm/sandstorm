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
        RenderTarget2D _rt = null;

        public DrawEngine(GraphicsDevice pGraphicsDevice, ContentManager pContentManager, SharedList pList)
        {
            this._graphicsDevice = pGraphicsDevice;
            this._contentManager = pContentManager;
            this._sharedList = pList;

            _spriteBatch = new SpriteBatch(this._graphicsDevice);
            _font = _contentManager.Load<SpriteFont>("font/FPSFont");
            _instancing = new Instancing(_graphicsDevice,_contentManager,_sharedList);

            _rt = new RenderTarget2D(_graphicsDevice, _graphicsDevice.PresentationParameters.BackBufferWidth,_graphicsDevice.PresentationParameters.BackBufferHeight, false,SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        }


        public void Update(GameTime pGameTime) //Update DrawEngine
        {
        }

        private void ShowTextureTopLeft(Texture2D pos)
        {
            SpriteBatch _spriteBatch = new SpriteBatch(_graphicsDevice);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.Default,
                RasterizerState.CullNone);
            _spriteBatch.Draw(pos, new Vector2(0, 0), Color.White);
            _spriteBatch.End();
        }
        
        public Texture2D Draw(Camera pCamera,Texture2D particles) //Draw all Particles
        {
            _graphicsDevice.SetRenderTarget(_rt); //Instanced Particles to Renderbuffer

            //Draw Particles
            RasterizerState prevRasterizerState = _graphicsDevice.RasterizerState;
            BlendState prevBlendState = _graphicsDevice.BlendState;

            ShowTextureTopLeft(particles);
            _instancing.Draw(pCamera, particles);
            
            _graphicsDevice.BlendState = prevBlendState;
            _graphicsDevice.RasterizerState = prevRasterizerState;

            _graphicsDevice.SetRenderTarget(null);

            return _rt; //Return Result
        }
    }
}
