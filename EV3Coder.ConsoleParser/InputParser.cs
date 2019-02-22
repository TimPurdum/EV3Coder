using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using EV3Coder.Core;
using Lego.Ev3.Desktop;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EV3Coder.ConsoleParser
{
    public class InputParser
    {
        public void Input(string inputString)
        {
            if (!string.IsNullOrEmpty(Code))
            {
                Code += Environment.NewLine + "            ";
            }
            Code += inputString;   
        }

        public string Code { get; set; } = "";

        public void Run()
        {
            var codeToCompile = @"
using System;
using EV3Coder.Core;
using Lego.Ev3.Desktop;

namespace CompilerCode
{
    public class ParserCode
    {
        public void Run()
        {
            var comm = new BluetoothCommunication(""COM4"");
            var controller = new BrickController(comm);
            controller.Connect();
            " + Code +
                @"
        }
    }
}";
            Console.WriteLine("Code: ");
            Console.WriteLine(codeToCompile);
            Console.WriteLine("Running...");
            var syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);
            
            var assemblyName = Path.GetRandomFileName();
            
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var legoPath = Path.GetDirectoryName(typeof(BluetoothCommunication).Assembly.Location);
            var coderPath = Path.GetDirectoryName(typeof(BrickController).Assembly.Location);
            
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Private.CoreLib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Private.CoreLib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Console.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),    
                MetadataReference.CreateFromFile(Path.Combine(legoPath, "Lego.Ev3.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coderPath, "EV3Coder.Core.dll"))
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