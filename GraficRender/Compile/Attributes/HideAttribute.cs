
namespace GraficRender.Compile.Attributes;

public class HideAttribute : AttributedModule
{
    public override void WriteToInfo(FunctionModel.FunctionInfo functionInfo)
    {
        functionInfo.Hide = true;
    }
}
