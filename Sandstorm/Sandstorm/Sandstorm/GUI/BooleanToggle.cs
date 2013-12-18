using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ruminate.GUI.Content;
using System.Reflection;
using Ruminate.GUI.Framework;

namespace Sandstorm.GUI
{
    class BooleanToggle
    {
        private String Text { get; set; }
        private bool Value { get; set; }
        private Label Label { get; set; }
        private CheckBox CheckBox { get; set; }
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
        public BooleanToggle(Widget parent, object propertyOwner, string propertyName, int posX, int posY)
        {
            //Set property owner
            PropertyOwner = propertyOwner;

            //Get property with given name from owner
            Property = PropertyOwner.GetType().GetProperty(propertyName);

            //Set label text
            Text = Property.Name;

            //Get initial value from property
            Value = (bool)Property.GetValue(PropertyOwner, null);

            //Create new label
            Label = new Label(posX, posY, Text);

            //Add label to parent
            parent.AddWidget(Label);

            //Create new slider
            CheckBox = new CheckBox(posX, posY + 20, "");

            //Add ValueChanged event handler
            CheckBox.OnToggle += OnToggle;
            CheckBox.OffToggle += OnToggle;
            
            //Add slider to parent
            parent.AddWidget(CheckBox);

            //Set slider initial value (must set after AddWidget!)
            CheckBox.SetToggle(Value);    
        }

        /// <summary>
        /// Handles slider ValueChanged event.
        /// </summary>
        /// <param name="widget"></param>
        public void OnToggle(Widget widget)
        {
            //Update value
            Value = !((CheckBox)widget).IsToggled;

            //Update connected property
            Property.SetValue(PropertyOwner, Value, null);
        }
    }
}
