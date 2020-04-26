using Microsoft.CodeAnalysis;

namespace Scar.CodeAnalysis
{
    public class NonAsyncMethodNameCodeFixProvider : BaseAsyncMethodNameCodeFixProvider
    {
        protected override string Title { get; } = "Remove Async from name";

        protected override string ReplaceText(SyntaxToken token) => token.Text.TrimEnd("Async") ?? string.Empty;
    }
}
