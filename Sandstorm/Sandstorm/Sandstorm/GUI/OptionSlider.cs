using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ruminate.Utils;
using Ruminate.GUI.Framework;
using Ruminate.GUI.Content;
using ParticleStormDLL;
using System.Reflection;

namespace Sandstorm.GUI
{

    public class OptionSlider //creates a Slider with Label, which modifies ParticleProperties
    {
        private String labelText = "";
        private float value = 0;
        private int posX = 0;
        private int posY = 0;

        private Widget[] w = null;

        Label _sliderLabel = null;
        Slider _slider = null;
        private Object OwnerObject = null;
        private PropertyInfo FieldInfo;
        private float Min {get; set;}
        private float Max {get; set;}


        public OptionSlider(int posX, int posY, int width, String labelText, float min, float max, Object ownerObject)
        {
            Min = min;
            Max = max;
            this.OwnerObject = ownerObject;
            this.posX = posX;
            this.posY = posY;
            this.labelText = labelText;

            FieldInfo = OwnerObject.GetType().GetProperty(labelText, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            this.value = (float)FieldInfo.GetValue(OwnerObject, null);
            _sliderLabel = new Label(posX, posY, labelText + " = " + value);
            _sliderLabel.Value = labelText + " = " + this.value;

            

            _slider = new Slider(posX + 200, posY, width, delegate(Widget slider)
            {
                this.value = min + (float)((Slider)slider).Value * max;
                _sliderLabel.Value = labelText + " = " + this.value;        
                FieldInfo.SetValue(OwnerObject, this.value, null);
            });

            this.w = new Widget[]{_sliderLabel, _slider};
        }

        public Widget[] getWidget()
        {
            return this.w;
        }
    }
}
