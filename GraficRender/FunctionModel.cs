using GraficRender.Compile.Attributes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraficRender;

public class FunctionModel
{
    public FunctionModel(MethodInfo method, Color color)
    {
        Color = color;
        Method = method;
    }

    public Color Color;

    public MethodInfo Method { get; }

    private bool? _hasDynamicArgument;
    public bool HasDynamicArgument => _hasDynamicArgument ??= Arguments.Any(parameter => parameter.ParameterType == typeof(float) && parameter.GetCustomAttribute<Compile.Attributes.DynamicParameter>() != null);

    private ParameterInfo[]? _arguments;
    public ParameterInfo[] Arguments => _arguments ??= Method.GetParameters();

    private List<VertexPositionColor> _calculatedPosition = new();

    public float Invoke(float x, float time)
    {
        return (float)Method.Invoke(null, HasDynamicArgument ? new object[] { x, time } : new object[] { x })!;
    }

    public List<VertexPositionColor> GetVertexBuffer(float time, int minValue, int maxValue, float step)
    {
        time = MathF.Round(time, 2);
        _calculatedPosition.Clear();
        for (float x = minValue; x < maxValue; x += step)
        {
            float y = MathF.Round(Invoke(x, time), 4);

            if (!float.IsNormal(y))
                continue;

            _calculatedPosition.Add(new VertexPositionColor { Color = Color, Position = new Vector3(x, y, 0f) });
        }

        return _calculatedPosition;
    }

    public float TakeDerivative(float x, float time)
    {
        return (Invoke(x + MainGame.Step, time) - Invoke(x, time)) / MainGame.Step;
    }

    public class FunctionInfo
    {
        public Color Color { get; set; }
        public List<DynamicParameter> DynamicParameters { get; set; } = new();
    }

    public class DynamicParameter
    {
        public float Min { get; }
        public float Max { get; }

        // x - (y * y / x)
        public float Calculate(float time)
        {
            if (time > Max)
            {
               return time - Max * MathF.Floor(Max / time);
            }

            return time;
        }
    }
}
