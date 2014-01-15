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
using System.Reflection;

namespace Sandstorm.GUI
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class HUD : DrawableGameComponent
    {
        public Gui _gui;
        SpriteFont Calibri20;
        SpriteFont Calibri16;
        Texture2D _ImageMap;
        string _Map;

        //Elements
        ParticleStorm _particleSystem = null;

        public HUD(Game game,ParticleStorm particleSystem) : base(game)
        {
            Calibri20 = Game.Content.Load<SpriteFont>("font\\Calibri20");
            Calibri16 = Game.Content.Load<SpriteFont>("font\\Calibri16");
            //_ImageMap = Game.Content.Load<Texture2D>("GUI\\ImageMap");
            //_Map = File.OpenText("Content\\GUI\\Map.txt").ReadToEnd();
            _ImageMap = Game.Content.Load<Texture2D>("GUI\\StormTheme");
            _Map = File.OpenText("Content\\GUI\\StormMap.txt").ReadToEnd();
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

        private void createOptionPanel(int width, int height)
        {
            Panel mainPanel = new Panel(width - 170, 0, 170, height);
            Label head = new Label(35, 15, "ParticleSystem") { Text = "Calibri20" };
            Panel optionPanel = new Panel(0, 50, 170, height-50);
            ScrollBars scrollBars = new ScrollBars();

            _gui.AddWidget(mainPanel);
            mainPanel.AddWidget(head);
            mainPanel.AddWidget(optionPanel);
            optionPanel.AddWidget(scrollBars);

            PropertyInfo[] properties = _particleSystem.ParticleProperties.GetType().GetProperties();
            int offset = 0;

            for (int i = 0; i < properties.Length; i++)
            {
                Console.WriteLine(properties[i].PropertyType);

                switch (properties[i].PropertyType.ToString())
                {
                    case "System.Single":
                        FloatSlider os = new FloatSlider(scrollBars,
                                                         _particleSystem.ParticleProperties,
                                                         properties[i].Name,
                                                         0, 
                                                         offset, 
                                                         145,
                                                         0.0f, 
                                                         100.0f);
                        offset += 40;
                        break;
                    case "Microsoft.Xna.Framework.Vector3":
                        Vector3Slider v3s = new Vector3Slider(scrollBars,
                                                              _particleSystem.ParticleProperties,
                                                              properties[i].Name,
                                                              0,
                                                              offset,
                                                              145,
                                                              new Vector3(-999, -999, -999),
                                                              new Vector3(999, 999, 999));
                        offset += 85;
                        break;
                    case "Microsoft.Xna.Framework.Color":
                        ColorSlider cs = new ColorSlider(scrollBars,
                                                         _particleSystem.ParticleProperties,
                                                         properties[i].Name,
                                                         0,
                                                         offset,
                                                         145,
                                                         new Color(0, 0, 0, 0),
                                                         new Color(255, 255, 255, 255));
                        offset += 100;
                        break;
                    case "System.Boolean":
                        BooleanToggle bt = new BooleanToggle(scrollBars,
                                                             _particleSystem.ParticleProperties,
                                                             properties[i].Name,
                                                             0,
                                                             offset);
                        offset += 40;
                        break;
                    
                } 
            }
        }

        private void initGui()
        {
            var skin = new Skin(_ImageMap, _Map);
            var calibri16 = new Text(Calibri16, Color.White);
            var calibri20 = new Text(Calibri20, Color.White);

            var testSkins = new[] { new Tuple<string, Skin>("Skin", skin) };
            var testTexts = new[] { new Tuple<string, Text>("Calibri16", calibri16), 
                                    new Tuple<string, Text>("Calibri20", calibri20)};

            int width = Game.Window.ClientBounds.Width;
            int height = Game.Window.ClientBounds.Height;

            _gui = new Gui(base.Game, skin, calibri16, testSkins, testTexts); //creat the GUI
            this.createOptionPanel(width, height);

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
