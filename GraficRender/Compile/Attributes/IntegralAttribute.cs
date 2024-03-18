namespace GraficRender.Compile.Attributes;

public class IntegralAttribute : AttributedModule
{
    public float IntegrateConstant { get; }
    public IntegralAttribute(float integrateConstant = 0)
    {
        IntegrateConstant = integrateConstant;
    }
    public override void WriteToInfo(FunctionModel.FunctionInfo functionInfo)
    {
        functionInfo.Type = FunctionType.Integral;
        functionInfo.IntegrateConstant = IntegrateConstant;
    }
}
