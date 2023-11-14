using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraficRender.Compile.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DynamicParameter : Attribute
    {
        public float Min { get; }
        public float Max { get; }

        public DynamicParameter(float min, float max)
        {
            Min = min; 
            Max = max;
        }

        public DynamicParameter()
        {
            Min = float.MinValue;
            Max = float.MaxValue;
        }
    }
}
