
namespace GraficRender.Compile.Attributes;

public class UpdateAttribute : AttributedModule
{
    public override void WriteToInfo(FunctionModel.FunctionInfo functionInfo)
    {
        functionInfo.ShouldUpdate = true;
    }
}
