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
        public Gui GUI { get; set; }

        private SpriteFont _Calibri20;
        private SpriteFont _Calibri16;
        private Texture2D _ImageMap;
        private string _Map;

        private Panel _MainMenu;
        private ScrollBars _MainContent;
        private List<Object> _Items;
        private bool _Visible;

        public int MenuOffset { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        public HUD(Game game) : base(game)
        {
            MenuOffset = 170;
            _Items = new List<Object>();
            _Calibri20 = Game.Content.Load<SpriteFont>("font\\Calibri20");
            _Calibri16 = Game.Content.Load<SpriteFont>("font\\Calibri16");
            _ImageMap = Game.Content.Load<Texture2D>("GUI\\StormThemePurple");
            _Map = File.OpenText("Content\\GUI\\StormMap.txt").ReadToEnd();       
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        public void InitGui()
        {
            _Visible = true;

            var skin = new Skin(_ImageMap, _Map);
            var calibri16 = new Text(_Calibri16, Color.White);
            var calibri20 = new Text(_Calibri20, Color.White);

            var testSkins = new[] { new Tuple<string, Skin>("Skin", skin) };
            var testTexts = new[] { new Tuple<string, Text>("Calibri16", calibri16), 
                                    new Tuple<string, Text>("Calibri20", calibri20)};
            int width = Game.Window.ClientBounds.Width;
            int height = Game.Window.ClientBounds.Height;

            GUI = new Gui(base.Game, skin, calibri16, testSkins, testTexts);

            CreateMainMenu(width, height);
            Hide();
        }

        private void CreateMainMenu(int width, int height)
        {
            _MainMenu = new Panel(width - MenuOffset, 0, 175, height);
            _MainContent = new ScrollBars();
            Label head = new Label(5, 15, "Options") { Text = "Calibri20" };
            Panel optionPanel = new Panel(0, 50, 170, height - 50);

            GUI.AddWidget(_MainMenu);
            _MainMenu.AddWidget(head);
            _MainMenu.AddWidget(optionPanel);
            optionPanel.AddWidget(_MainContent);

            for (int i=0; i<_Items.Count; i++)
            {
                string label = _Items[i].GetType().Name.Substring(0, _Items[i].GetType().Name.IndexOf("Properties"));

                Widget subMenu = CreateSubMenu(width, height, _Items[i], label);

                _MainContent.AddWidget(new Button(0, 30 * i, 170-15, label, buttonEvent: delegate(Widget widget)
                {
                    HideWidget(_MainMenu);
                    ShowWidget(subMenu);
                }));
            }
        }

        private Widget CreateSubMenu(int width, int height, Object item, string label)
        {
            Panel subPanel = new Panel(width - MenuOffset, 0, 175, height);
            Label head = new Label(5, 15, label) { Text = "Calibri20" };
            Panel optionPanel = new Panel(0, 50, 170, height - 50);
            ScrollBars scrollBars = new ScrollBars();

            GUI.AddWidget(subPanel);
            subPanel.AddWidget(head);
            subPanel.AddWidget(optionPanel);
            optionPanel.AddWidget(scrollBars);

            PropertyInfo[] properties = item.GetType().GetProperties();

            for (int i = 0, offset = 0; i < properties.Length; i++)
            {
                switch (properties[i].PropertyType.ToString())
                {
                    case "System.Int32":
                        IntSlider iS = new IntSlider(scrollBars,
                                                     item,
                                                     properties[i].Name,
                                                     0,
                                                     offset,
                                                     145,
                                                     0,
                                                     2000);
                        offset += 40;
                        break;
                    case "System.Single":
                        FloatSlider oS = new FloatSlider(scrollBars,
                                                         item,
                                                         properties[i].Name,
                                                         0,
                                                         offset,
                                                         145,
                                                         0.0f,
                                                         100.0f);
                        offset += 40;
                        break;
                    case "Microsoft.Xna.Framework.Vector3":
                        Vector3Slider v3S = new Vector3Slider(scrollBars,
                                                              item,
                                                              properties[i].Name,
                                                              0,
                                                              offset,
                                                              145,
                                                              new Vector3(-999, -999, -999),
                                                              new Vector3(999, 999, 999));
                        offset += 85;
                        break;
                    case "Microsoft.Xna.Framework.Color":
                        ColorSlider cS = new ColorSlider(scrollBars,
                                                         item,
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
                                                             item,
                                                             properties[i].Name,
                                                             0,
                                                             offset);
                        offset += 40;
                        break;
                    case "Microsoft.Xna.Framework.Graphics.Texture2D":
                        break;
                }
            }

            return subPanel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void AddSubMenu(Object item)
        {
            _Items.Add(item);
            _Visible = true;
            InitGui();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Show()
        {
            if (!_Visible)
            {
                ShowWidget(_MainMenu);
                _Visible = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Hide()
        {
            if (_Visible)
            {
                foreach(Widget widget in GUI.Widgets)
                {
                    HideWidget(widget);
                }
                _Visible = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="widget"></param>
        private void HideWidget(Widget widget)
        {
            foreach (Widget child in widget.Children)
            {
                HideWidget(child);
                child.Visible = false;
            }
            widget.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="widget"></param>
        private void ShowWidget(Widget widget)
        {
            //Widget[] children = widget.Children.ToArray();
            foreach (Widget child in widget.Children)
            {
                ShowWidget(child);
                child.Visible = true;
            }
            widget.Visible = true;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (GUI != null) 
                GUI.Update();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (GUI != null) 
                GUI.Draw();
            base.Draw(gameTime);
        }

        public void OnResize()
        {
            this.InitGui();
            GUI.Resize();
        }
    }
}
