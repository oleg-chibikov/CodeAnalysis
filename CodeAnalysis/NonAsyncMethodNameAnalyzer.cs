using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Scar.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public class NonAsyncMethodNameAnalyzer : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "Scar_Async_002";
        static readonly LocalizableString Title = "Non async method must not end with Async";
        static readonly LocalizableString MessageFormat = "Non async method '{0}' ends with Async";
        static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Constants.Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            context.RegisterSymbolAction(AsyncMethodValidator, SymbolKind.Method);
        }

        static void AsyncMethodValidator(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;
            if (!methodSymbol.IsRenameApplicable())
            {
                return;
            }

            if (!methodSymbol.IsAsync && methodSymbol.Name.EndsWith("Async", StringComparison.Ordinal))
            {
                foreach (var location in methodSymbol.Locations)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, location, methodSymbol.Name));
                }
            }
        }
    }
}
