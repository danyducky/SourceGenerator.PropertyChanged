using System;
using SourceGenerator.PropertyChanged.Infrastructure.Indent;

namespace SourceGenerator.PropertyChanged.Abstractions.Models;

/// <summary>
/// Syntax tree node.
/// </summary>
public abstract class SyntaxMetadata : ICloneable
{
    /// <summary>
    /// Build metadata.
    /// </summary>
    /// <param name="writer">Indent writer.</param>
    /// <returns>String metadata representation.</returns>
    public abstract string Build(IndentWriter writer);

    /// <summary>
    /// Make a shallow copy.
    /// </summary>
    /// <returns>Shallow copy.</returns>
    public virtual object Clone() => this.MemberwiseClone();
}
