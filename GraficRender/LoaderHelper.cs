using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GraficRender.Compile;
using GraficRender.Compile.Attributes;
using Microsoft.CSharp;
using Microsoft.Xna.Framework;

namespace GraficRender;

public static class LoaderHelper
{
    private readonly static string Example = """
            using System;
            using System.Collections;
            using System.Collections.Generic;
            using System.Numerics;

            using GraficRender.Compile.Attributes;

            using Microsoft.Xna.Framework;

            public class CompilerForFunc 
            {
                {code}
            }
            """;
    private static Assembly? currentassembly;
    public static Assembly CurrentAssembly => currentassembly ??= GetAssembly();

    private static Assembly GetAssembly()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string file in Directory.EnumerateFiles("Functions"))
        {
            stringBuilder.Append(File.ReadAllText(file));
        }

        RoslynCompiler compiler = new RoslynCompiler(Example.Replace("{code}", stringBuilder.ToString()), Array.Empty<Type>());
        return compiler.Compile();
    }

    public static Dictionary<string, FunctionModel> LoadAll()
    {
        Type type = CurrentAssembly.GetType("CompilerForFunc");
        Dictionary<string, FunctionModel> result = new();
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            if (CheckMethod(method, out Color color))
                result.Add(method.Name, new FunctionModel(method, color));
        }
        return result;
    }

    private static bool CheckMethod(MethodInfo method, out Color color)
    {
        color = Color.White;
        if (method.ReturnType != typeof(IEnumerable<float>) && method.ReturnType != typeof(float))
        {
            Console.WriteLine($"Invalid method: {method.Name}");
            return false;
        }
        if (!method.GetParameters().Any(parameter => parameter.Name == "x" && parameter.ParameterType == typeof(float)))
        {
            Console.WriteLine($"Invalid method: {method.Name}");
            return false;
        }
        if (method.GetCustomAttribute<ColorAttribute>() is ColorAttribute colorAttribute)
        {
            color = colorAttribute.Color;
        }
        return true;
    }
}
