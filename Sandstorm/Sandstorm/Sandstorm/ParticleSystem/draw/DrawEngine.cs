using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Sandstorm.ParticleSystem.structs;
using Dhpoware;

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
        
        private Billboard _billboard;
        private Texture2D billboardTexture;
        private Effect billboardEffect;


        public DrawEngine(GraphicsDevice pGraphicsDevice, ContentManager pContentManager, Camera pCamera,SharedList pList)
        {
            this._camera = pCamera;
            this._graphicsDevice = pGraphicsDevice;
            this._contentManager = pContentManager;
            this._sharedList = pList;

            _spriteBatch = new SpriteBatch(this._graphicsDevice);
            _font = _contentManager.Load<SpriteFont>("font/FPSFont");


            billboardTexture = _contentManager.Load<Texture2D>("tex/particle"); 
            billboardEffect = _contentManager.Load<Effect>(@"fx\Billboard");
            billboardEffect.CurrentTechnique = billboardEffect.Techniques["BillboardingCameraAligned"];
        }


        public void Update(GameTime pGameTime) //Update DrawEngine
        {
            _fpsCounter.Update();
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

            _sharedList.getParticles().ToArray();
                       
            _billboard = new Billboard(_graphicsDevice,
                                      _sharedList.getParticles().ToArray(),
                                      1.0f,
                                      1.0f,
                                      0.0f,
                                      0.0f);


            RasterizerState prevRasterizerState = _graphicsDevice.RasterizerState;
            BlendState prevBlendState = _graphicsDevice.BlendState;

            // First pass:
            // Render the non-transparent pixels of the billboards and store
            // their depths in the depth buffer.

            
        float BILLBOARD_ANIM_FPS = 24.0f;
        float BILLBOARD_ANIM_TIME_MIN = 0.0f;
        float BILLBOARD_ANIM_TIME_MAX = MathHelper.TwoPi;
        float BILLBOARD_ANIM_SCALE = 0.15f;
        float billboardAnimationTime = BILLBOARD_ANIM_TIME_MIN;
        Vector2 billboardSize = new Vector2(2.0f, 2.0f);


            billboardEffect.Parameters["world"].SetValue(Matrix.Identity);
            billboardEffect.Parameters["view"].SetValue(_camera.ViewMatrix);
            billboardEffect.Parameters["projection"].SetValue(_camera.ProjMatrix);
            billboardEffect.Parameters["billboardSize"].SetValue(billboardSize);
            billboardEffect.Parameters["colorMap"].SetValue(billboardTexture);
            billboardEffect.Parameters["animationTime"].SetValue(billboardAnimationTime);
            billboardEffect.Parameters["animationScaleFactor"].SetValue(BILLBOARD_ANIM_SCALE);
            billboardEffect.Parameters["alphaTestDirection"].SetValue(1.0f);

            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphicsDevice.RasterizerState = RasterizerState.CullNone;

            _billboard.Draw(_graphicsDevice, billboardEffect);

            // Second pass:
            // Render the transparent pixels of the billboards.
            // Disable depth buffer writes to ensure that the depth values from
            // the first pass are used instead.

            billboardEffect.Parameters["alphaTestDirection"].SetValue(-1.0f);

            _graphicsDevice.BlendState = BlendState.NonPremultiplied;
            _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;


            _billboard.Draw(_graphicsDevice, billboardEffect);

            // Restore original states.

            _graphicsDevice.BlendState = prevBlendState;
            _graphicsDevice.RasterizerState = prevRasterizerState;
        }
    }
}
