using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ruminate.GUI.Content;
using System.Reflection;
using Ruminate.GUI.Framework;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace Sandstorm.GUI
{
    class Vector3Slider
    {
        private String Text { get; set; }
        private Vector3 Value { get; set; }
        private Label Label { get; set; }
        private Slider SliderX { get; set; }
        private Slider SliderY { get; set; }
        private Slider SliderZ { get; set; }
        private Vector3 Min { get; set; }
        private Vector3 Max { get; set; }
        private object PropertyOwner { get; set; }
        private PropertyInfo Property { get; set; }

        /// <summary>
        /// Creates an instance of Vector3Slider.
        /// </summary>
        /// <param name="parent">The parent where the Slider is append to</param>
        /// <param name="propertyOwner">The owner of the connected property</param>
        /// <param name="propertyOwner">The name of the connected property</param>
        /// <param name="posX">The X position of the OptionSlider</param>
        /// <param name="posY">The Y position of the OptionSlider</param>
        /// <param name="width">The width of the OptionSlider</param>
        /// <param name="min">The minimal value of the OptionSlider</param>
        /// <param name="max">The maximal value of the OptionSlider</param>
        public Vector3Slider(Widget parent, object propertyOwner, string propertyName, int posX, int posY, int width, Vector3 min, Vector3 max)
        {
            //Set property owner
            PropertyOwner = propertyOwner;

            //Get property with given name from owner
            Property = PropertyOwner.GetType().GetProperty(propertyName);

            //Set label text
            Text = Property.Name;

            //Get initial value from property
            Value = (Vector3)Property.GetValue(PropertyOwner, null);

            //Set the minimal slider value
            Min = min;

            //Set the maximal slider value
            Max = max;

            //Create new label
            Label = new Label(posX, posY, Text);

            //Add label to parent
            parent.AddWidget(Label);

            //Create new slider
            SliderX = new Slider(posX, posY + 20, width);
            SliderY = new Slider(posX, posY + 35, width);
            SliderZ = new Slider(posX, posY + 50, width);

            //Add ValueChanged event handler
            SliderX.ValueChanged += OnValueChangedX;
            SliderY.ValueChanged += OnValueChangedY;
            SliderZ.ValueChanged += OnValueChangedZ;

            //Add slider to parent
            parent.AddWidgets(new Widget[]{SliderX, SliderY, SliderZ});

            //Set slider initial value (must set after AddWidget!)
            SliderX.Value = Value.X / Max.X;
            SliderY.Value = Value.Y / Max.Y;
            SliderZ.Value = Value.Z / Max.Z; 
        }

        /// <summary>
        /// Handles x-component slider ValueChanged event.
        /// </summary>
        /// <param name="widget"></param>
        public void OnValueChangedX(Widget widget)
        {
            //Update value
            Value = new Vector3(Min.X + ((Slider)widget).Value * Max.X, Value.Y, Value.Z);

            //Update label
            Label.Value = Text;

            //Update connected property
            Property.SetValue(PropertyOwner, Value, null);
        }

        /// <summary>
        /// Handles y-component slider ValueChanged event.
        /// </summary>
        /// <param name="widget"></param>
        public void OnValueChangedY(Widget widget)
        {
            //Update value
            Value = new Vector3(Value.X, Min.Y + ((Slider)widget).Value * Max.Y, Value.Z);

            //Update label
            Label.Value = Text;

            //Update connected property
            Property.SetValue(PropertyOwner, Value, null);
        }

        /// <summary>
        /// Handles z-component slider ValueChanged event.
        /// </summary>
        /// <param name="widget"></param>
        public void OnValueChangedZ(Widget widget)
        {
            //Update value
            Value = new Vector3(Value.X, Value.Y, Min.Z + ((Slider)widget).Value * Max.Z);

            //Update label
            Label.Value = Text;

            //Update connected property
            Property.SetValue(PropertyOwner, Value, null);
        }
    }
}
