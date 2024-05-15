using System;

namespace GraficRender.Compile.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class TitleAttribute : Attribute
{
    public TitleAttribute() { }
}
