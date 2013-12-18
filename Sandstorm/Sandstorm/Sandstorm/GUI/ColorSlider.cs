using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ruminate.GUI.Content;
using Microsoft.Xna.Framework;
using System.Reflection;
using Ruminate.GUI.Framework;

namespace Sandstorm.GUI
{
    class ColorSlider
    {
        private String Text { get; set; }
        private Color Value { get; set; }
        private Label Label { get; set; }
        private Slider SliderR { get; set; }
        private Slider SliderG { get; set; }
        private Slider SliderB { get; set; }
        private Slider SliderA { get; set; }
        private Color Min { get; set; }
        private Color Max { get; set; }
        private object PropertyOwner { get; set; }
        private PropertyInfo Property { get; set; }

        /// <summary>
        /// Creates an instance of ColorSlider.
        /// </summary>
        /// <param name="parent">The parent where the Slider is append to</param>
        /// <param name="propertyOwner">The owner of the connected property</param>
        /// <param name="propertyOwner">The name of the connected property</param>
        /// <param name="posX">The X position of the OptionSlider</param>
        /// <param name="posY">The Y position of the OptionSlider</param>
        /// <param name="width">The width of the OptionSlider</param>
        /// <param name="min">The minimal value of the OptionSlider</param>
        /// <param name="max">The maximal value of the OptionSlider</param>
        public ColorSlider(Widget parent, object propertyOwner, string propertyName, int posX, int posY, int width, Color min, Color max)
        {
            //Set property owner
            PropertyOwner = propertyOwner;

            //Get property with given name from owner
            Property = PropertyOwner.GetType().GetProperty(propertyName);

            //Set label text
            Text = Property.Name;

            //Get initial value from property
            Value = (Color)Property.GetValue(PropertyOwner, null);

            //Set the minimal slider value
            Min = min;

            //Set the maximal slider value
            Max = max;

            //Create new label
            Label = new Label(posX, posY, Text);

            //Add label to parent
            parent.AddWidget(Label);

            //Create new slider
            SliderR = new Slider(posX, posY + 20, width);
            SliderG = new Slider(posX, posY + 35, width);
            SliderB = new Slider(posX, posY + 50, width);
            SliderA = new Slider(posX, posY + 65, width);

            //Add ValueChanged event handler
            SliderR.ValueChanged += OnValueChangedR;
            SliderG.ValueChanged += OnValueChangedG;
            SliderB.ValueChanged += OnValueChangedB;
            SliderA.ValueChanged += OnValueChangedA;

            //Add slider to parent
            parent.AddWidgets(new Widget[]{SliderR, SliderG, SliderB, SliderA});

            //Set slider initial value (must set after AddWidget!)
            SliderR.Value = Value.R / Max.R;
            SliderG.Value = Value.G / Max.G;
            SliderB.Value = Value.B / Max.B;
            SliderA.Value = Value.A / Max.A;
        }

        /// <summary>
        /// Handles r-component slider ValueChanged event.
        /// </summary>
        /// <param name="widget"></param>
        public void OnValueChangedR(Widget widget)
        {
            //Update value
            Value = Value = new Color(Min.R + ((Slider)widget).Value * Max.R, Value.G, Value.B, Value.A);

            //Update label
            Label.Value = Text;

            //Update connected property
            Property.SetValue(PropertyOwner, Value, null);
        }

        /// <summary>
        /// Handles g-component slider ValueChanged event.
        /// </summary>
        /// <param name="widget"></param>
        public void OnValueChangedG(Widget widget)
        {
            //Update value
            Value = new Color(Value.R, Min.G + ((Slider)widget).Value * Max.G, Value.B, Value.A);

            //Update label
            Label.Value = Text;

            //Update connected property
            Property.SetValue(PropertyOwner, Value, null);
        }

        /// <summary>
        /// Handles b-component slider ValueChanged event.
        /// </summary>
        /// <param name="widget"></param>
        public void OnValueChangedB(Widget widget)
        {
            //Update value
            Value = new Color(Value.R, Value.G, Min.B + ((Slider)widget).Value * Max.B, Value.A);

            //Update label
            Label.Value = Text;

            //Update connected property
            Property.SetValue(PropertyOwner, Value, null);
        }

        /// <summary>
        /// Handles a-component slider ValueChanged event.
        /// </summary>
        /// <param name="widget"></param>
        public void OnValueChangedA(Widget widget)
        {
            //Update value
            Value = new Color(Value.R, Value.G, Value.B, Min.A + ((Slider)widget).Value * Max.A);

            //Update label
            Label.Value = Text;

            //Update connected property
            Property.SetValue(PropertyOwner, Value, null);
        }
    }
}
