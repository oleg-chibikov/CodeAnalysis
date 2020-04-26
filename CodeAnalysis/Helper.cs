using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Scar.CodeAnalysis
{
    static class Helper
    {
        public static bool IsRenameApplicable(this ISymbol methodSymbol)
        {
            var thisTypeAssembly = methodSymbol.ContainingType.ContainingAssembly;
            var interfaces = methodSymbol.ContainingType.Interfaces.Where(x => x.MemberNames.Contains(methodSymbol.Name, StringComparer.Ordinal));
            if (interfaces.Any(x => x.ContainingAssembly.Identity != thisTypeAssembly.Identity))
            {
                return false;
            }

            if (methodSymbol.IsAbstract || methodSymbol.IsOverride || methodSymbol.Name.Equals("Main", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}
