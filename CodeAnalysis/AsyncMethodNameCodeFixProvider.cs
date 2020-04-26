using Microsoft.CodeAnalysis;

namespace Scar.CodeAnalysis
{
    public class AsyncMethodNameCodeFixProvider : BaseAsyncMethodNameCodeFixProvider
    {
        protected override string Title { get; } = "Add Async to name";

        protected override string ReplaceText(SyntaxToken token) => token.Text + "Async";
    }
}
