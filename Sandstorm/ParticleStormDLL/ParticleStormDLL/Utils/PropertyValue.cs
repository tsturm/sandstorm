using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParticleStormDLL.Utils
{
    public class PropertyValue<T>
    {
        public T Value { get; set; }
        public T Max { get; set; }
        public T Min { get; set; }
        public T Step { get; set; }

        public PropertyValue(T value, T max, T min, T step)
        {
            Value = value;
            Max = max;
            Min = min;
            Step = step;
        }
    }
}
