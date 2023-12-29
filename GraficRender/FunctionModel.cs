using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GraficRender;

public class FunctionModel
{
    public FunctionModel(MethodInfo method)
    {
        Method = method;
    }

    public FunctionInfo Info { get; } = new(Color.White);

    public MethodInfo Method { get; }

    private List<VertexPositionColor> _calculatedPosition = new();

    public float Invoke(float x)
    {
        return (float)Method.Invoke(null, new object[] { x })!;
    }

    public List<VertexPositionColor> GetVertexBuffer(int minValue, int maxValue, float step)
    {
        _calculatedPosition.Clear();
        bool derivative = Info.Derivative;
        for (float x = minValue; x < maxValue; x += step)
        {
            float y = derivative ? TakeDerivative(x) : Invoke(x);
            y = MathF.Round(y, 4);

            if (float.IsNaN(y))
                continue;

            _calculatedPosition.Add(new VertexPositionColor { Color = Info.Color, Position = new Vector3(x, y, 0f) });
        }

        return _calculatedPosition;
    }

    public float TakeDerivative(float x)
    {
        return (Invoke(x + MainGame.Step) - Invoke(x)) / MainGame.Step;
    }

    public class FunctionInfo
    {
        public FunctionInfo(Color color)
        {
            Color = color;
        }

        public Color Color { get; set; }
        public bool ShouldUpdate { get; set; } = false;
        public bool Derivative { get; set; } = false;
    }
}
