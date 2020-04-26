using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using Scar.CodeAnalysis.Test.Helpers;
using CodeFixVerifier = Scar.CodeAnalysis.Test.Verifiers.CodeFixVerifier;

namespace Scar.CodeAnalysis.Test
{
    [Parallelizable]
    public class AsyncMethodNameAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void ProducesNoDiagnosticsForEmptyString()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void ProducesNoDiagnosticsForInterfaces()
        {
            const string test = @"
    namespace ConsoleApplication1
    {
        interface TypeName
        {
            async Task Method();

            async Task Method2();
        }
    }";
            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void ProducesNoDiagnosticsForOverridenMethods()
        {
            const string test = @"
    namespace ConsoleApplication1
    {
        class TypeName : BaseType
        {
            override async Task Method()
            {
                await Task.CompletedTask;
            }

            override async Task Method2()
            {
                await Task.CompletedTask;
            }
        }
    }";
            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void AppliesDiagnosticsAndFix()
        {
            const string test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            async Task Method()
            {
                await Task.CompletedTask;
            }

            async Task Method2()
            {
                await Task.CompletedTask;
            }
        }
    }";
            VerifyCSharpDiagnostic(test, GetExpectedResult("Method", 6, 24), GetExpectedResult("Method2", 11, 24));

            const string fixedTest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            async Task MethodAsync()
            {
                await Task.CompletedTask;
            }

            async Task Method2Async()
            {
                await Task.CompletedTask;
            }
        }
    }";
            VerifyCSharpFix(test, fixedTest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new AsyncMethodNameCodeFixProvider();
        }

        protected override CodeFixProvider GetBasicCodeFixProvider()
        {
            return new AsyncMethodNameCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AsyncMethodNameAnalyzer();
        }

        protected override DiagnosticAnalyzer GetBasicDiagnosticAnalyzer()
        {
            return new AsyncMethodNameAnalyzer();
        }

        static DiagnosticResult GetExpectedResult(string methodName, int line, int column)
        {
            var expected = new DiagnosticResult
            {
                Id = AsyncMethodNameAnalyzer.DiagnosticId,
                Message = $"Async method '{methodName}' does not end with Async",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", line, column),
                }
            };
            return expected;
        }
    }
}
