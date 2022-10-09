using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraficRender.Compile.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ColorAttribute : Attribute
    {
        public ColorAttribute(Color color)
        {
            Color = color;
        }

        public Color Color { get; set; }
    }
}
