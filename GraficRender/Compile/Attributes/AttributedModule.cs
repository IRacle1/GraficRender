using System;

namespace GraficRender.Compile.Attributes;

public abstract class AttributedModule : Attribute
{
    public abstract void WriteToInfo(FunctionModel.FunctionInfo functionInfo);
}
