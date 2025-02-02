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
/// Analyzer for partial class based on <see cref="ITypeSymbol"/> code analysis API.
/// </summary>
public class ClassAnalyzer : ISyntaxAnalyzer<ITypeSymbol, ClassAnalysis>
{
    private readonly OptionsManager optionsManager;

    private readonly ISyntaxAnalyzer<ITypeSymbol, InterfaceAnalysis> propertyChangedAnalyzer;
    private readonly ISyntaxAnalyzer<ITypeSymbol, InterfaceAnalysis> propertyChangingAnalyzer;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="optionsManager">Options manager.</param>
    public ClassAnalyzer(OptionsManager optionsManager)
    {
        this.optionsManager = optionsManager;

        this.propertyChangedAnalyzer = InterfaceAnalyzerFactory.Create(optionsManager, InterfaceType.PropertyChanged);
        this.propertyChangingAnalyzer = InterfaceAnalyzerFactory.Create(optionsManager, InterfaceType.PropertyChanging);
    }

    /// <inheritdoc/>
    public ClassAnalysis Analyze(ITypeSymbol symbol, SemanticModel semanticModel, IDiagnosticsScope scope)
    {
        var analysis = GetMembersAnalysis(symbol, semanticModel, scope);

        return new ClassAnalysis
        {
            Symbol = symbol,
            PropertyChanged = propertyChangedAnalyzer.Analyze(symbol, semanticModel, scope),
            PropertyChanging = propertyChangingAnalyzer.Analyze(symbol, semanticModel, scope),
            Properties = analysis.OfType<PropertyAnalysis>().ToList(),
            Fields = analysis.OfType<FieldAnalysis>().ToList(),
        };
    }

    private IReadOnlyCollection<MemberAnalysis> GetMembersAnalysis(
        ITypeSymbol symbol,
        SemanticModel semanticModel,
        IDiagnosticsScope scope)
    {
        // Possible multiple iterations through symbol members,
        // so cast them to fixed arrays.
        var members = SymbolUtils
            .GetMembers(symbol)
            .ToArray();

        var fields = members.OfType<IFieldSymbol>()
            .Where(field => field.CanBeReferencedByName)
            .Where(field => FieldUtils.FollowConvention(field.Name, optionsManager.FieldOptions))
            .ToArray();
        var properties = members
            .OfType<IPropertySymbol>()
            .ToArray();

        var fieldAnalysis = new List<FieldAnalysis>();
        var fieldAnalyzer = new FieldAnalyzer(fields, properties, optionsManager.FieldOptions);
        foreach (var field in fields)
        {
            var analysis = fieldAnalyzer.Analyze(field, semanticModel, scope);
            fieldAnalysis.Add(analysis);
        }

        var propertyAnalysis = new List<PropertyAnalysis>();
        var propertyAnalyzer = new PropertyAnalyzer(fieldAnalysis, optionsManager.FieldOptions);
        foreach (var property in properties)
        {
            var analysis = propertyAnalyzer.Analyze(property, semanticModel, scope);
            propertyAnalysis.Add(analysis);
        }

        return propertyAnalysis.Concat<MemberAnalysis>(fieldAnalysis).ToArray();
    }
}
