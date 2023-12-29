using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraficRender.Compile.Attributes
{
    public class DerivativeAttribute : AttributedModule
    {
        public override void WriteToInfo(FunctionModel.FunctionInfo functionInfo)
        {
            functionInfo.Derivative = true;
        }
    }
}
