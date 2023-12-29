using System;

namespace GraficRender.Compile.Attributes;

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
