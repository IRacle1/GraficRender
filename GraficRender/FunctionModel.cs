using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraficRender;

public class FunctionModel
{
    public FunctionModel(MethodInfo method)
    {
        Method = method;
    }

    public MethodInfo Method { get; }

    private bool? _shouldUpdate;
    public bool ShouldUpdate => _shouldUpdate ??= Arguments.Any(parameter => parameter.Name is "t" or "time" && parameter == typeof(float));

    private bool? _multiPoints;
    public bool IsMultiPoints => _multiPoints ??= Method.ReturnType == typeof(IEnumerable<float>);

    private Type[]? _arguments;
    public Type[] Arguments => _arguments ??= Method.GetParameters().Select(p => p.ParameterType).ToArray();

    public float Invoke(float x, float? time)
    {
        return (float)Method.Invoke(null, ShouldUpdate ? new object[] { x, time.Value } : new object[] { x });
    }

    public IEnumerable<float> InvokeMulti(float x, float? time)
    {
        return (IEnumerable<float>)Method.Invoke(null, ShouldUpdate ? new object[] { x, time.Value } : new object[] { x });
    }

    public List<VertexPositionColor> GetVertexBuffer(float time, Color color, int minValue, int maxValue, float step)
    {
        time = MathF.Round(time, 2);
        List<VertexPositionColor> list = new();
        for (float x = minValue; x < maxValue; x += step)
        {
            float y = MathF.Round(Invoke(x, time), 4);

            if (!float.IsNormal(y))
                continue;

            list.Add(new VertexPositionColor { Color = color, Position = new Vector3(x, y, 0f) });
        }

        return list;
    }

    public List<VertexPositionColor> GetVertexBufferMulti(float time, Color color, int minValue, int maxValue, float step)
    {
        time = MathF.Round(time, 2);
        List<VertexPositionColor> list = new();
        for (float x = minValue; x < maxValue; x += step)
        {
            foreach (float res in InvokeMulti(x, time))
            {
                float y = MathF.Round(res, 4);

                if (!float.IsNormal(y))
                    continue;

                list.Add(new VertexPositionColor { Color = color, Position = new Vector3(x, y, 0f) });
            }
        }

        return list;
    }
}
