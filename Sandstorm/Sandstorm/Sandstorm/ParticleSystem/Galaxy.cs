using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandstorm.ParticleSystem.draw;
using Sandstorm.ParticleSystem.physic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Sandstorm.ParticleSystem
{
    public class Galaxy
    {
        private DrawEngine _drawEngine = null; //Draw-Engine
        private PhysicEngine _physicEngine = null; //PhysicEngine
        private SharedList _sharedList = new SharedList(); //SharedList of Particles

        private Camera _camera;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;

        public Galaxy(GraphicsDevice pGraphicsDevice, ContentManager pContentManager, Camera pCamera)
        {
            _camera = pCamera;
            _graphicsDevice = pGraphicsDevice;
            _contentManager = pContentManager;

            _drawEngine = new DrawEngine(pGraphicsDevice,pContentManager,pCamera,_sharedList);
            _physicEngine = new PhysicEngine(_sharedList);
        }


        public void Update(GameTime pGameTime)
        {
            _drawEngine.Update(pGameTime);
            _physicEngine.Update(pGameTime);
        }

        public void Draw()
        {
            _physicEngine.Draw();
            _drawEngine.Draw(_drawEngine.getFPS(), _physicEngine.getFPS());
        }
    }
}
