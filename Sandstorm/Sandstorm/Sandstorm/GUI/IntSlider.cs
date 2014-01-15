using System;
using Ruminate.GUI.Framework;
using Ruminate.GUI.Content;
using System.Globalization;
using System.Reflection;

namespace Sandstorm.GUI
{
    public class IntSlider
    {
        private String Text { get; set; }
        private int Value { get; set; } 
        private Label Label { get; set; }
        private Slider Slider { get; set; }
        private int Min {get; set;}
        private int Max {get; set;}
        private object PropertyOwner { get; set; }
        private PropertyInfo Property { get; set; }

        /// <summary>
        /// Creates an instance of FloatSlider.
        /// </summary>
        /// <param name="parent">The parent where the Slider is append to</param>
        /// <param name="propertyOwner">The owner of the connected property</param>
        /// <param name="propertyOwner">The name of the connected property</param>
        /// <param name="posX">The X position of the OptionSlider</param>
        /// <param name="posY">The Y position of the OptionSlider</param>
        /// <param name="width">The width of the OptionSlider</param>
        /// <param name="min">The minimal value of the OptionSlider</param>
        /// <param name="max">The maximal value of the OptionSlider</param>
        public IntSlider(Widget parent, object propertyOwner, string propertyName, int posX, int posY, int width, int min, int max)
        {
            //Set property owner
            PropertyOwner = propertyOwner;

            //Get property with given name from owner
            Property = PropertyOwner.GetType().GetProperty(propertyName);

            //Set label text
            Text = Property.Name;

            //Get initial value from property
            Value = (int)Property.GetValue(PropertyOwner, null);

            //Set the minimal slider value
            Min = min;

            //Set the maximal slider value
            Max = max;

            //Create new label
            Label = new Label(posX, posY, Text + " [" + Value.ToString("n1", CultureInfo.InvariantCulture) + "]");

            //Add label to parent
            parent.AddWidget(Label);

            //Create new slider
            Slider = new Slider(posX, posY + 20, width);

            //Add ValueChanged event handler
            Slider.ValueChanged += OnValueChanged;

            //Add slider to parent
            parent.AddWidget(Slider);

            //Set slider initial value (must set after AddWidget!)
            Slider.Value = Value / Max;           
        }

        /// <summary>
        /// Handles slider ValueChanged event.
        /// </summary>
        /// <param name="widget"></param>
        public void OnValueChanged(Widget widget)
        {
            //Update value
            Value = (int)(Min + ((Slider)widget).Value * Max);

            //Update label
            Label.Value = Text + " [" + Value.ToString("n1", CultureInfo.InvariantCulture) + "]";

            //Update connected property
            Property.SetValue(PropertyOwner, Value, null);
        }
    }
}
