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

    public static Dictionary<string, FunctionModel> LoadAll(bool forceCompile = false)
    {
        Dictionary<string, FunctionModel> result = new();

        CurrentAssembly = GetAssembly(forceCompile);

        if (CurrentAssembly == null)
        {
            return result;
        }

        foreach (FieldInfo field in CurrentAssembly.GetType("CompilerForFunc")!.GetFields())
        {
            if (field.GetCustomAttribute<DynamicParameterAttribute>() is not DynamicParameterAttribute param)
                continue;

            DynamicParameters.Add(new DynamicParameter(field, param.Min, param.Max));
        }

        Type type = MainType = CurrentAssembly.GetType("CompilerForFunc")!;

        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            if (CheckMethod(method))
            {
                FunctionModel model = new(method);
                foreach (AttributedModule attribute in method.GetCustomAttributes<AttributedModule>())
                {
                    attribute.WriteToInfo(model.Info);
                }
                result.Add(method.Name, model);
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
