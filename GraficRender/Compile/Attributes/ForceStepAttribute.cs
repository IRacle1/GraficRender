
namespace GraficRender.Compile.Attributes;

public class ForceStepAttribute : AttributedModule
{
    public float Step { get; }

    public ForceStepAttribute(float step)
    {
        Step = step;
    }

    public override void WriteToInfo(FunctionModel.FunctionInfo functionInfo)
    {
        functionInfo.Step = Step;
    }
}
