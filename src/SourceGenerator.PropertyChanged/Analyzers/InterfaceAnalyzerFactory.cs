﻿using System;
using System.ComponentModel;
using SourceGenerator.PropertyChanged.Infrastructure.Options;
using SourceGenerator.PropertyChanged.Models.Analyzers;

namespace SourceGenerator.PropertyChanged.Analyzers;

/// <summary>
/// Factory for <see cref="InterfaceAnalyzer"/>.
/// </summary>
internal static class InterfaceAnalyzerFactory
{
    /// <summary>
    /// Create an instance of <see cref="InterfaceAnalyzer"/>.
    /// </summary>
    /// <param name="optionsManager">Options manager.</param>
    /// <param name="type">Interface type.</param>
    /// <returns>Interface analyzer.</returns>
    public static InterfaceAnalyzer Create(OptionsManager optionsManager, InterfaceType type) => type switch
    {
        InterfaceType.PropertyChanged => new InterfaceAnalyzer(optionsManager.PropertyChangedOptions.MethodNames, typeof(INotifyPropertyChanged)),
        InterfaceType.PropertyChanging => new InterfaceAnalyzer(optionsManager.PropertyChangingOptions.MethodNames, typeof(INotifyPropertyChanging)),
        _ => throw new ArgumentOutOfRangeException("Invalid interface type."),
    };
}
