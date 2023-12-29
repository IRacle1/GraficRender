using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraficRender.Compile.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DynamicParameterAttribute : Attribute
    {
        public float Min { get; }
        public float Max { get; }

        public DynamicParameterAttribute(float min, float max)
        {
            Min = min; 
            Max = max;
        }

        public DynamicParameterAttribute()
        {
            Min = -10;
            Max = 10;
        }
    }
}
