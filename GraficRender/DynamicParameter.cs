using System;
using System.Reflection;

namespace GraficRender
{
    public class DynamicParameter
    {
        public DynamicParameter(FieldInfo fieldInfo, float min, float max)
        {
            Name = fieldInfo.Name;
            Min = min;
            Max = max;
            FieldInfo = fieldInfo;
        }
        public float Min { get; set; }
        public float Max { get; set; }

        public string Name { get; set; }

        public FieldInfo FieldInfo { get; }

        // x - (y * y / x)
        public float Calculate(float time)
        {
            if (time > Max)
            {
                return time - Max * MathF.Floor(Max / time);
            }

            return time;
        }

        public float CalculateSin(float time)
        {
            float middle = (Max + Min) / 2;
            float delta = Max - middle;
            return delta * MathF.Sin(4 * MathF.PI * time) + middle;
        }

        public void Set(float time)
        {
            FieldInfo.SetValue(null, CalculateSin(time));
        }
    }
}
