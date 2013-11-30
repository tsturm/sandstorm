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
        Slider _slider;
        Label _sliderLabel;

        public HUD(Game game) : base(game)
        {
            _SpriteFont = Game.Content.Load<SpriteFont>("GUI\\Texture");
            _ImageMap = Game.Content.Load<Texture2D>("GUI\\ImageMap");
            _Map = File.OpenText("Content\\GUI\\Map.txt").ReadToEnd();
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

        private void initGui()
        {
            var skin = new Skin(_ImageMap, _Map);
            var text = new Text(_SpriteFont, Color.White);

            var testSkins = new[] { new Tuple<string, Skin>("Skin", skin) };
            var testTexts = new[] { new Tuple<string, Text>("Text", text) };

            int width = Game.Window.ClientBounds.Width;
            int height = Game.Window.ClientBounds.Height;
            _gui = new Gui(base.Game, skin, text, testSkins, testTexts)
            {
                Widgets = new Widget[] {
                             _sliderLabel = new Label((int)(0.8*width), (int)(0.8*height)+30, "Value = 0"),
                              _slider = new Slider((int)(0.8*width), (int)(0.8*height), (int)(0.2*width), delegate(Widget slider)
                             {
                                _sliderLabel.Value = "Value = " + ((Slider)slider).Value;
                            }),

                          }
            };

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
