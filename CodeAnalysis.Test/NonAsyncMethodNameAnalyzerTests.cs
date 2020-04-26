using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using Scar.CodeAnalysis.Test.Helpers;
using CodeFixVerifier = Scar.CodeAnalysis.Test.Verifiers.CodeFixVerifier;

namespace Scar.CodeAnalysis.Test
{
    [Parallelizable]
    public class NonAsyncMethodNameAnalyzerTests : CodeFixVerifier
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
            Task MethodAsync();

            Task Method2Async();
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
            override async Task MethodAsync()
            {
                await Task.CompletedTask;
            }

            override async Task Method2Async()
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
            void MethodAsync()
            {
            }

            void Method2Async()
            {
            }
        }
    }";
            VerifyCSharpDiagnostic(test, GetExpectedResult("MethodAsync", 6, 18), GetExpectedResult("Method2Async", 10, 18));

            const string fixedTest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Method()
            {
            }

            void Method2()
            {
            }
        }
    }";
            VerifyCSharpFix(test, fixedTest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new NonAsyncMethodNameCodeFixProvider();
        }

        protected override CodeFixProvider GetBasicCodeFixProvider()
        {
            return new NonAsyncMethodNameCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new NonAsyncMethodNameAnalyzer();
        }

        protected override DiagnosticAnalyzer GetBasicDiagnosticAnalyzer()
        {
            return new NonAsyncMethodNameAnalyzer();
        }

        static DiagnosticResult GetExpectedResult(string methodName, int line, int column)
        {
            var expected = new DiagnosticResult
            {
                Id = NonAsyncMethodNameAnalyzer.DiagnosticId,
                Message = $"Non async method '{methodName}' ends with Async",
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
