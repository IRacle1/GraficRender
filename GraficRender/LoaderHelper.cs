using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
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
            //using GraficRender.Compile.Helpers;

            using Microsoft.Xna.Framework;

            public class CompilerForFunc 
            {
                {code}
            }
            """;

    public static Assembly? CurrentAssembly { get; set; }

    public static string FilePath => $"Functions\\cashedassembly{DateTime.Now:d}.dll";

    private static Assembly? GetAssembly(bool forceCompile = false)
    {
        if (!forceCompile && File.Exists(FilePath))
        {
            using var stream = File.OpenRead(FilePath);
            return AssemblyLoadContext.Default.LoadFromStream(stream);
        }
        StringBuilder stringBuilder = new();
        foreach (string file in Directory.EnumerateFiles("Functions", "*.cs"))
        {
            stringBuilder.Append(File.ReadAllText(file));
        }

        RoslynCompiler compiler = new RoslynCompiler(Example.Replace("{code}", stringBuilder.ToString()), Array.Empty<Type>());
        return compiler.Compile();
    }

    public static Dictionary<string, FunctionModel> LoadAll(bool forceCompile = false)
    {
        Dictionary<string, FunctionModel> result = new();

        CurrentAssembly = GetAssembly(forceCompile);

        if (CurrentAssembly == null)
        {
            return result;
        }

        Type type = CurrentAssembly.GetType("CompilerForFunc")!;

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
        if (method.ReturnType != typeof(float))
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
