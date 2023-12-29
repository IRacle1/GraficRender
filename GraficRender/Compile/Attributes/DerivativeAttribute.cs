namespace GraficRender.Compile.Attributes;

public class DerivativeAttribute : AttributedModule
{
    public override void WriteToInfo(FunctionModel.FunctionInfo functionInfo)
    {
        functionInfo.Derivative = true;
    }
}
