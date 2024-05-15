using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using GraficRender.Compile;
using GraficRender.Compile.Attributes;

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
    public static Type? MainType { get; set; }

    public static List<DynamicParameter> DynamicParameters { get; } = new();

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

    public static Dictionary<int, FunctionModel> LoadAll(bool forceCompile = false)
    {
        if (!Directory.Exists("Functions"))
        {
            Directory.CreateDirectory("Functions");
            using StreamWriter stream = new(File.Create($"Functions/parabola.cs"));
            stream.Write("""
                    [Title]
                    public static string Title = "Temp1";

                    [DynamicParameter(-1, 1)]
                    public static float a;

                    [Color(0, 100, 100)]
                    [Update]
                    public static float SinDerv(float x)
                    {
                    	return a * MathF.Sin(x);
                    }

                    public static float Parabola(float x) 
                    {
                       	return x * x;
                    }
                    """);
        }

        Dictionary<int, FunctionModel> result = new();

        CurrentAssembly = GetAssembly(forceCompile);

        if (CurrentAssembly == null)
        {
            return result;
        }

        Type type = MainType = CurrentAssembly.GetType("CompilerForFunc")!;

        foreach (FieldInfo field in type.GetFields())
        {
            if (field.GetCustomAttribute<TitleAttribute>() is not null && field.GetValue(null) is string title)
            {
                MainGame.Instance.Window.Title = title;
            }
            if (field.GetCustomAttribute<DynamicParameterAttribute>() is not DynamicParameterAttribute param)
                continue;

            DynamicParameters.Add(new DynamicParameter(field, param.Min, param.Max));
        }

        int id = 1;

        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            if (CheckMethod(method))
            {
                FunctionModel model = new(method);

                foreach (AttributedModule attribute in method.GetCustomAttributes<AttributedModule>())
                {
                    attribute.WriteToInfo(model.Info);
                }

                if (!model.Info.Ignore)
                    result.Add(id++, model);
            }
        }

        return result;
    }

    private static bool CheckMethod(MethodInfo method)
    {
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

        return true;
    }
}
