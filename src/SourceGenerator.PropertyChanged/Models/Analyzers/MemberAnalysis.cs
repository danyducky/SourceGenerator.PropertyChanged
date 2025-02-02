using SourceGenerator.PropertyChanged.Abstractions.Analyzers;
using SourceGenerator.PropertyChanged.Models.Metadata;

namespace SourceGenerator.PropertyChanged.Models.Analyzers;

/// <summary>
/// Member analysis.
/// </summary>
public abstract record MemberAnalysis : ISyntaxAnalysis
{
    /// <summary>
    /// Member name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Member modifier.
    /// </summary>
    public string Modifier { get; set; } = MemberModifiers.Public;

    /// <summary>
    /// Member type.
    /// </summary>
    public string Type { get; set; }
}
