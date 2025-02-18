﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.CodeAnalysis;
using SourceGenerator.PropertyChanged.Abstractions.Syntax;
using SourceGenerator.PropertyChanged.Infrastructure.Options;
using SourceGenerator.PropertyChanged.Models.Analyzers;
using SourceGenerator.PropertyChanged.Models.Metadata;
using SourceGenerator.PropertyChanged.Models.Nodes;
using SourceGenerator.PropertyChanged.Utils;

namespace SourceGenerator.PropertyChanged.Builders;

/// <summary>
/// Partial class builder based on <see cref="ClassAnalysis"/>.
/// </summary>
public class ClassBuilder : ISyntaxBuilder<ClassMetadata, ClassAnalysis>
{
    private readonly static ISyntaxBuilder<PropertyMetadata, InterfaceAnalysis> propertyChangedPropertyBuilder
        = new InterfacePropertyBuilder(propertyName: "PropertyChanged", typeof(PropertyChangedEventHandler));

    private readonly static ISyntaxBuilder<PropertyMetadata, InterfaceAnalysis> propertyChangingPropertyBuilder
        = new InterfacePropertyBuilder(propertyName: "PropertyChanging", typeof(PropertyChangingEventHandler));

    private readonly OptionsManager optionsManager;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="optionsManager">Options manager.</param>
    public ClassBuilder(OptionsManager optionsManager)
    {
        this.optionsManager = optionsManager;
    }

    /// <inheritdoc/>
    public ClassMetadata Build(ClassAnalysis analysis)
    {
        var members = new List<MemberMetadata>();
        var methods = new List<MethodMetadata>();

        var declaredMethods = SymbolUtils.GetMembers(analysis.Symbol).OfType<IMethodSymbol>();
        var declaredMethodNames = declaredMethods.Select(method => method.Name);

        var propertyChanged = GetInterfaceMethodInvocation(
            analysis.Symbol,
            analysis.PropertyChanged,
            analysis =>
            {
                var property = propertyChangedPropertyBuilder.Build(analysis);
                members.Add(property);
                return property;
            },
            (analysis, invocation) =>
            {
                invocation.Arguments.Add("this");
                invocation.Arguments.Add("new PropertyChangedEventArgs(propertyName)");

                var propertyChangedMethodName = optionsManager.PropertyChangedOptions
                    .MethodNames
                    .Except(declaredMethodNames)
                    .First();

                var method = new InterfaceMethodBuilder(
                    methodName: propertyChangedMethodName,
                    invocation).Build(analysis);
                methods.Add(method);
                return method;
            });

        var propertyChanging = GetInterfaceMethodInvocation(
            analysis.Symbol,
            analysis.PropertyChanging,
            analysis =>
            {
                var property = propertyChangingPropertyBuilder.Build(analysis);
                members.Add(property);
                return property;
            },
            (analysis, invocation) =>
            {
                invocation.Arguments.Add("this");
                invocation.Arguments.Add("new PropertyChangingEventArgs(propertyName)");

                var propertyChangingMethodName = optionsManager.PropertyChangingOptions
                    .MethodNames
                    .Except(declaredMethodNames)
                    .First();

                var method = new InterfaceMethodBuilder(
                    methodName: propertyChangingMethodName,
                    invocation).Build(analysis);
                methods.Add(method);
                return method;
            });

        var additionalMembers = GetMembersMetadata(
                analysis.Properties,
                analysis.Fields,
                propertyChanged,
                propertyChanging);
        members.AddRange(additionalMembers);

        return new ClassMetadata()
        {
            Name = analysis.Name,
            Namespace = analysis.Namespace,
            Modifier = analysis.Modifier,
            Methods = methods,
            Members = members,
        };
    }

    private static InvocationMethodMetadata? GetInterfaceMethodInvocation(
        ITypeSymbol symbol,
        InterfaceAnalysis analysis,
        Func<InterfaceAnalysis, PropertyMetadata> buildProperty,
        Func<InterfaceAnalysis, InvocationMethodMetadata, MethodMetadata> buildMethod)
    {
        if (!analysis.HasInterface)
        {
            return default;
        }

        var propertyChanged = InvocationMethodMetadata.Instance;

        if (analysis.MethodSymbol is not null)
        {
            propertyChanged.Name = analysis.MethodSymbol.Name;
        }
        else
        {
            var shouldBuildProperty = analysis.EventSymbol == null ||
                SymbolUtils.ContainsBaseList(symbol, analysis.EventSymbol.ContainingSymbol);

            var propertyInvocation = new InvocationMethodMetadata
            {
                Name = shouldBuildProperty
                    ? buildProperty(analysis).Name
                    : analysis.EventSymbol.Name,
                IsNullable = true,
                IsDelegate = true,
            };
            var method = buildMethod(analysis, propertyInvocation);
            propertyChanged.Name = method.Name;
        }

        return propertyChanged;
    }

    private IEnumerable<MemberMetadata> GetMembersMetadata(
        IEnumerable<PropertyAnalysis> properties,
        IEnumerable<FieldAnalysis> fields,
        InvocationMethodMetadata? propertyChanged = null,
        InvocationMethodMetadata? propertyChanging = null)
    {
        var propertyNames = properties.Select(property => PropertyUtils.GetFieldName(
            property.Name,
            optionsManager.FieldOptions));

        var fieldsToAnalyze = fields
            .Where(field => !propertyNames.Contains(field.Name))
            .Where(field => !field.DoNotNotify);

        var propertyBuilder = new PropertyBuilder(
            optionsManager.FieldOptions,
            propertyChanged,
            propertyChanging);

        return fieldsToAnalyze
            .Select(propertyBuilder.Build)
            .Cast<MemberMetadata>()
            .ToList();
    }
}
