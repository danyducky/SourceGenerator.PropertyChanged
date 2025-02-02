using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace SourceGenerator.PropertyChanged.Abstractions.Diagnostics;

/// <summary>
/// Diagnostics manager.
/// </summary>
public interface IDiagnostics
{
    /// <summary>
    /// List of diagnostics.
    /// </summary>
    ImmutableArray<Diagnostic> Diagnostics { get; }
}
