using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System;
using Microsoft.CodeAnalysis.Emit;
using System.Net.Http.Headers;
using System.Numerics;

namespace GraficRender.Compile;

public class RoslynCompiler
{
    readonly CSharpCompilation _compilation;

    Assembly _generatedAssembly;

    public RoslynCompiler(string code, Type[] typesToReference)
    {
        List<MetadataReference> refs = typesToReference.Select(h => MetadataReference.CreateFromFile(h.Assembly.Location) as MetadataReference).ToList();

        //some default refs
        refs.Add(MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll")));
        refs.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

        //generate syntax tree from code and config compilation options
        var syntax = CSharpSyntaxTree.ParseText(code);
        var options = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            allowUnsafe: true,
            optimizationLevel: OptimizationLevel.Release);

        _compilation = CSharpCompilation.Create(Guid.NewGuid().ToString(), new List<SyntaxTree> { syntax }, refs, options);
    }

    public Assembly Compile()
    {
        if(_generatedAssembly != null)
            return _generatedAssembly;
        using MemoryStream ms = new MemoryStream();
        EmitResult result = _compilation.Emit(ms);
        if (!result.Success)
        {
            var compilationErrors = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error)
                .ToList();
            if (compilationErrors.Any())
            {
                var firstError = compilationErrors.First();
                var errorNumber = firstError.Id;
                var errorDescription = firstError.GetMessage();
                var firstErrorMessage = $"{errorNumber}: {errorDescription};";
                var exception = new Exception($"Compilation failed, first error is: {firstErrorMessage}");
                compilationErrors.ForEach(e => { if (!exception.Data.Contains(e.Id)) exception.Data.Add(e.Id, e.GetMessage()); });
                throw exception;
            }
        }

        ms.Seek(0, SeekOrigin.Begin);

        return AssemblyLoadContext.Default.LoadFromStream(ms);
    }
}