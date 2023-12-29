using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraficRender.Compile.Attributes
{
    public abstract class AttributedModule : Attribute
    {
        public abstract void WriteToInfo(FunctionModel.FunctionInfo functionInfo);
    }
}
