using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using SourceGenerator.PropertyChanged.Abstractions.Analyzers;
using SourceGenerator.PropertyChanged.Abstractions.Diagnostics;
using SourceGenerator.PropertyChanged.Infrastructure.Options;
using SourceGenerator.PropertyChanged.Models.Analyzers;
using SourceGenerator.PropertyChanged.Utils;

namespace SourceGenerator.PropertyChanged.Analyzers;

/// <summary>
/// Analyzer of <see cref="IPropertySymbol"/>.
/// </summary>
internal class PropertyAnalyzer : ISyntaxAnalyzer<IPropertySymbol, PropertyAnalysis>
{
    private readonly IEnumerable<FieldAnalysis> fields;
    private readonly FieldOptions fieldOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fields">Field analysis.</param>
    /// <param name="fieldOptions">Field options.</param>
    public PropertyAnalyzer(IEnumerable<FieldAnalysis> fields, FieldOptions fieldOptions)
    {
        this.fields = fields;
        this.fieldOptions = fieldOptions;
    }

    /// <inheritdoc/>
    public PropertyAnalysis Analyze(IPropertySymbol symbol, SemanticModel semanticModel, IDiagnosticsScope scope)
    {
        var propertyAnalysis = new PropertyAnalysis()
        {
            Name = symbol.Name,
            Type = SymbolUtils.GetType(symbol),
            Modifier = SymbolUtils.GetModifier(symbol),
        };

        var propertyFieldName = PropertyUtils.GetFieldName(symbol.Name, fieldOptions);
        var backingField = fields.FirstOrDefault(field => field.Name == propertyFieldName);
        if (backingField != null)
        {
            backingField.AssociatedProperty = propertyAnalysis;
            propertyAnalysis.BackingField = backingField;
        }

        return propertyAnalysis;
    }
}
