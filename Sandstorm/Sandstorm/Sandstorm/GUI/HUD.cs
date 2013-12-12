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
using Ruminate.GUI.Framework;
using Ruminate.GUI.Content;
using System.IO;
using Ruminate.Utils;
using System.Globalization;
using ParticleStormDLL;

namespace Sandstorm.GUI
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class HUD : DrawableGameComponent
    {
        public Gui _gui;
        SpriteFont _SpriteFont;
        Texture2D _ImageMap;
        string _Map;        

        //Elements
        ParticleStorm _particleSystem = null;

        public HUD(Game game,ParticleStorm particleSystem) : base(game)
        {
            _SpriteFont = Game.Content.Load<SpriteFont>("GUI\\Texture");
            _ImageMap = Game.Content.Load<Texture2D>("GUI\\ImageMap");
            _Map = File.OpenText("Content\\GUI\\Map.txt").ReadToEnd();
            _particleSystem = particleSystem;
            initGui();                      
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        private static int padding = 10;
        private static int OptionPadding = 40;

        private Panel createOptionPanel(int width,int height)
        {
            Panel p = new Panel(0, 0, (int)(width * 0.5), (int)(height * 0.5)); //Option Panel

            //creating different options
            String[] options = new String[] { "LifeTimeMin", "LifeTimeMax", "StartSizeMin", "StartSizeMax" };
            for (int i = 0; i < options.Length; i++) // create the Options Window
            {
                OptionSlider os = new OptionSlider(p.Area.Left + padding, p.Area.Top + padding + i * OptionPadding, (int)(width * 0.2), options[i],_particleSystem);
                p.AddWidgets(os.getWidget());
            }

            //closeOptionButton
            Button closeButton = new Button(p.Area.Left + padding, p.Area.Bottom - padding - 50, 50, "Close", delegate //Close Button of Option Windows
            {
                p.Visible = false;
                //_particleSystem.ParticleProperties.StartSizeMin = val;
            });
            p.AddWidget(closeButton);
            p.Visible = false; //default not visible
            return p;
        }

        private void initGui()
        {
            var skin = new Skin(_ImageMap, _Map);
            var text = new Text(_SpriteFont, Color.White);

            var testSkins = new[] { new Tuple<string, Skin>("Skin", skin) };
            var testTexts = new[] { new Tuple<string, Text>("Text", text) };

            int width = Game.Window.ClientBounds.Width;
            int height = Game.Window.ClientBounds.Height;

            Panel optionPanel = this.createOptionPanel(width,height);

            Button openButton = new Button(width - 70, 0, 50, "Settings", delegate
            {
                optionPanel.Visible = true;
                //_particleSystem.ParticleProperties.StartSizeMin = val;
            });
            
            _gui = new Gui(base.Game, skin, text, testSkins, testTexts); //creat the GUI
            _gui.AddWidget(optionPanel); //add Option Panel
            _gui.AddWidget(openButton); //add openSettings Button

           
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (_gui != null) 
                _gui.Update();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_gui != null) 
                _gui.Draw();
            base.Draw(gameTime);
        }


        public void OnResize()
        {
            this.initGui();
            _gui.Resize();
        }
    }
}
