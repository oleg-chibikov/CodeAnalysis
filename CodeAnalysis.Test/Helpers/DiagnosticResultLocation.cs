using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Scar.CodeAnalysis.Test.Helpers
{
    /// <summary>
    /// Location where the diagnostic appears, as determined by path, line number, and column number.
    /// </summary>
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "No need")]
    public readonly struct DiagnosticResultLocation
    {
        public DiagnosticResultLocation(string path, int line, int column)
        {
            if (line < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(line), "line must be >= -1");
            }

            if (column < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(column), "column must be >= -1");
            }

            Path = path;
            Line = line;
            Column = column;
        }

        public string Path { get; }

        public int Line { get; }

        public int Column { get; }
    }

    /// <summary>
    /// Struct that stores information about a Diagnostic appearing in a source.
    /// </summary>
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "No need")]
    public struct DiagnosticResult
    {
        IReadOnlyCollection<DiagnosticResultLocation> _locations;

        public IReadOnlyCollection<DiagnosticResultLocation> Locations
        {
            get => _locations ??= Array.Empty<DiagnosticResultLocation>();

            set => _locations = value;
        }

        public DiagnosticSeverity Severity { get; set; }

        public string Id { get; set; }

        public string Message { get; set; }

        public string Path => Locations.Count > 0 ? Locations.First().Path : string.Empty;

        public int Line => Locations.Count > 0 ? Locations.First().Line : -1;

        public int Column => Locations.Count > 0 ? Locations.First().Column : -1;
    }
}
