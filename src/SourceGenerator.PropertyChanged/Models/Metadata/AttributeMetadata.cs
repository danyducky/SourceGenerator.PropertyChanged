using System.Text;
using SourceGenerator.PropertyChanged.Abstractions.Models;
using SourceGenerator.PropertyChanged.Infrastructure.Indent;
using SourceGenerator.PropertyChanged.Models.Analyzers;

namespace SourceGenerator.PropertyChanged.Models.Metadata;

/// <summary>
/// Contains attribute metadata.
/// </summary>
public class AttributeMetadata : SyntaxMetadata
{
    private readonly AttributeAnalysis analysis;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="analysis">Attribute analysis.</param>
    public AttributeMetadata(AttributeAnalysis analysis)
    {
        this.analysis = analysis;
    }

    /// <inheritdoc />
    public override string Build(IndentWriter writer)
    {
        var builder = new StringBuilder();

        builder.Append("[");
        builder.Append($"{analysis.ContainingNamespace}.{analysis.Syntax}");
        builder.Append("]");

        var attribute = builder.ToString();

        return writer.Append(attribute).ToString();
    }
}
