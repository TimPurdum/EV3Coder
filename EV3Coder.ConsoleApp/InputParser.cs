using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace EV3Coder.ConsoleApp
{
    public class InputParser
    {
        public void Input(string inputString)
        {
            if (!string.IsNullOrEmpty(Code))
            {
                Code += Environment.NewLine;
            }
            Code += inputString;   
        }

        public string Code { get; set; } = "";

        public void Run()
        {
            var codeToCompile = @"
using System;

namespace CompilerCode
{
    public class ParserCode
    {
        public void Run()
        {
            " + Code +
                @"
        }
    }
}";
            Console.WriteLine("Code: ");
            Console.WriteLine(codeToCompile);
            var syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);
            
            var assemblyName = Path.GetRandomFileName();
            
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Private.CoreLib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Private.CoreLib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Console.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll"))
            };
            //adding the core dll containing object and other classes

            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic => 
                        diagnostic.IsWarningAsError || 
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in failures)
                    {
                        Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    
                    var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    var type= assembly.GetType("CompilerCode.ParserCode");
                    var instance = assembly.CreateInstance("CompilerCode.ParserCode");
                    var meth = type.GetMember("Run").First() as MethodInfo;
                    if (meth != null) meth.Invoke(instance, new object[0]);
                }
            }
        }
    }
}