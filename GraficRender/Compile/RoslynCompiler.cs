using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System;
using Microsoft.CodeAnalysis.Emit;

using GraficRender.Compile.Attributes;
using Microsoft.Xna.Framework;

namespace GraficRender.Compile;

public class RoslynCompiler
{
    readonly CSharpCompilation _compilation;

    private Assembly? _generatedAssembly;

    public RoslynCompiler(string content)
    {
        List<MetadataReference> refs = new();

        var rtPath = Path.GetDirectoryName(typeof(object).Assembly.Location) + Path.DirectorySeparatorChar;

        refs.Add(MetadataReference.CreateFromFile(rtPath + "System.Private.CoreLib.dll"));
        refs.Add(MetadataReference.CreateFromFile(rtPath + "System.Runtime.dll"));

        //some default refs
        //refs.Add(MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location)!, "System.Runtime.dll")));
        //refs.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        refs.Add(MetadataReference.CreateFromFile(typeof(ColorAttribute).Assembly.Location));
        refs.Add(MetadataReference.CreateFromFile(typeof(Color).Assembly.Location));

        //generate syntax tree from code and config compilation options
        var syntax = CSharpSyntaxTree.ParseText(content);
        var options = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            allowUnsafe: true,
            optimizationLevel: OptimizationLevel.Release);

        _compilation = CSharpCompilation.Create(Guid.NewGuid().ToString(), new List<SyntaxTree> { syntax }, refs, options);
    }

    public Assembly? Compile()
    {
        if(_generatedAssembly != null)
            return _generatedAssembly;
        using MemoryStream ms = new();
        EmitResult result = _compilation.Emit(ms);
        if (!result.Success)
        {
            var firstError = result.Diagnostics.First(diagnostic => diagnostic.IsWarningAsError ||
                                                                    diagnostic.Severity == DiagnosticSeverity.Error);
            var errorNumber = firstError.Id;
            var errorDescription = firstError.GetMessage();
            var firstErrorMessage = $"{errorNumber}: {errorDescription};";
            Console.WriteLine($"Compilation failed, first error is: {firstErrorMessage}");
            return null;
        }

        using var fileStream = File.OpenWrite(LoaderHelper.FilePath);

        ms.Seek(0, SeekOrigin.Begin);

        ms.CopyTo(fileStream);

        ms.Seek(0, SeekOrigin.Begin);

        return _generatedAssembly = AssemblyLoadContext.Default.LoadFromStream(ms);
    }
}