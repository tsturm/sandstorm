using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ruminate.Utils;
using Ruminate.GUI.Framework;
using Ruminate.GUI.Content;
using ParticleStormDLL;

namespace Sandstorm.GUI
{
    public class OptionSlider //creates a Slider with Label, which modifies ParticleProperties
    {
        private String labelText = "";

        private int value = 0;
        private int posX = 0;
        private int posY = 0;

        private Widget[] w = null;

        Label _sliderLabel = null;
        Slider _slider = null;
        private ParticleStorm _particleSystem = null;

        public OptionSlider(int posX, int posY, int width, String labelText, ParticleStorm _particleSystem)
        {
            this._particleSystem = _particleSystem;
            this.posX = posX;
            this.posY = posY;
            this.labelText = labelText;
            _sliderLabel = new Label(posX, posY, labelText + " = " + value);
            _slider = new Slider(posX + 200, posY, width, delegate(Widget slider)
            {
                                 this.value = (int)(((Slider)slider).Value*100);
                                 _sliderLabel.Value = labelText + " = " + this.value;
                                 _particleSystem.ParticleProperties.StartSizeMin = this.value;
                                 _particleSystem.ParticleProperties.StartSizeMax = this.value;
            });
            this.w = new Widget[]{_sliderLabel,_slider};
        }

        public Widget[] getWidget()
        {
            return this.w;
        }
    }
}
