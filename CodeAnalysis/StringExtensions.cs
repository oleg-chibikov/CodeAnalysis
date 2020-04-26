using System;

namespace Scar.CodeAnalysis
{
    static class StringExtensions
    {
        public static string? TrimEnd(this string? input, string suffixToRemove, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove, comparisonType))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }

            return input;
        }
    }
}
