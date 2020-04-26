using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace Scar.CodeAnalysis
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BaseAsyncMethodNameCodeFixProvider))]
    [Shared]
    public abstract class BaseAsyncMethodNameCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AsyncMethodNameAnalyzer.DiagnosticId);

        protected abstract string Title { get; }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                var diagnosticSpan = diagnostic.Location.SourceSpan;

                // Register a code action that will invoke the fix.
                context.RegisterCodeFix(CodeAction.Create(Title, cancellationToken => FixAsync(context.Document, diagnosticSpan, cancellationToken), Title), diagnostic);
            }

            return Task.CompletedTask;
        }

        protected abstract string ReplaceText(SyntaxToken token);

        async Task<Solution?> FixAsync(Document document, TextSpan span, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var token = root.FindToken(span.Start);

            if (!(token.Parent is MethodDeclarationSyntax methodDeclaration))
            {
                return null;
            }

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var symbol = semanticModel?.GetDeclaredSymbol(methodDeclaration, cancellationToken);
            if (symbol == null)
            {
                return null;
            }

            var project = document.Project;
            if (project == null)
            {
                return null;
            }

            var solution = document.Project.Solution;
            if (solution == null)
            {
                return null;
            }

            var optionSet = solution.Workspace.Options;
            var newName = ReplaceText(token);
            return await Renamer.RenameSymbolAsync(solution, symbol, newName, optionSet, cancellationToken).ConfigureAwait(false);
        }
    }
}
