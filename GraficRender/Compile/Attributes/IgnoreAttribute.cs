
namespace GraficRender.Compile.Attributes;

public class IgnoreAttribute : AttributedModule
{
    public IgnoreAttribute()
    {
    }

    public override void WriteToInfo(FunctionModel.FunctionInfo functionInfo)
    {
        functionInfo.Ignore = true;
    }
}
