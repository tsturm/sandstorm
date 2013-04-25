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

        private Effect _bbEffect;
        private Texture2D _particleTexture;
        private VertexBuffer _particleVertexBuffer;



        public DrawEngine(GraphicsDevice pGraphicsDevice, ContentManager pContentManager, Camera pCamera,SharedList pList)
        {
            this._camera = pCamera;
            this._graphicsDevice = pGraphicsDevice;
            this._contentManager = pContentManager;
            this._sharedList = pList;

            _spriteBatch = new SpriteBatch(this._graphicsDevice);
            _font = _contentManager.Load<SpriteFont>("font/FPSFont");


            _bbEffect = _contentManager.Load<Effect>("fx/bbEffect");
            _particleTexture = _contentManager.Load<Texture2D>("tex/particle");
        }


        public void Update(GameTime pGameTime) //Update DrawEngine
        {
            _fpsCounter.Update(pGameTime);
        }

        public int getFPS()
        {
            return _fpsCounter.getFrames();
        }


        private void CreateBillboardVerticesFromList(List<Particle> particleList)
        {
            VertexPositionTexture[] billboardVertices = new VertexPositionTexture[particleList.Count * 6];
            int i = 0;
            foreach (Particle p in particleList)
            {
                Vector3 pos = p.getPosition();
                billboardVertices[i++] = new VertexPositionTexture(pos, new Vector2(0, 0));
                billboardVertices[i++] = new VertexPositionTexture(pos, new Vector2(1, 0));
                billboardVertices[i++] = new VertexPositionTexture(pos, new Vector2(1, 1));

                billboardVertices[i++] = new VertexPositionTexture(pos, new Vector2(0, 0));
                billboardVertices[i++] = new VertexPositionTexture(pos, new Vector2(1, 1));
                billboardVertices[i++] = new VertexPositionTexture(pos, new Vector2(0, 1));
            }
            _particleVertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionTexture),billboardVertices.Length, BufferUsage.WriteOnly);
            _particleVertexBuffer.SetData(billboardVertices);
        }


        public void Draw(int pFPSDraw,int pFPSPhysic) //Draw all Particles
        {         
            
            _spriteBatch.Begin();
            String s = string.Format("Particles={0}, DrawEngineFPS={1}, PhysicEngineFPS={2}", _sharedList.getParticles().Count, pFPSDraw, pFPSPhysic);
        
            Vector2 screenSize = new Vector2(_graphicsDevice.Viewport.Height, _graphicsDevice.Viewport.Width);
            Vector2 pos = _font.MeasureString(s);
            
            _spriteBatch.DrawString(_font, s, new Vector2(0,0), Color.White);
            _spriteBatch.End();
            



            CreateBillboardVerticesFromList(_sharedList.getParticles());
            _bbEffect.CurrentTechnique = _bbEffect.Techniques["CylBillboard"];
            _bbEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
            _bbEffect.Parameters["xView"].SetValue(_camera.ViewMatrix);
            _bbEffect.Parameters["xProjection"].SetValue(_camera.ProjMatrix);
            _bbEffect.Parameters["xCamPos"].SetValue(_camera.getCameraPos());
            _bbEffect.Parameters["xAllowedRotDir"].SetValue(new Vector3(0, 1, 0));
            _bbEffect.Parameters["xBillboardTexture"].SetValue(_particleTexture);

            _graphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (EffectPass pass in _bbEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.SetVertexBuffer(_particleVertexBuffer);
                _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _particleVertexBuffer.VertexCount / 3);
            }
            _graphicsDevice.BlendState = BlendState.Opaque; 
        }
    }
}
