using SourceGenerator.PropertyChanged.Models.Descriptors;

namespace SourceGenerator.PropertyChanged.Diagnostics;

/// <summary>
/// Contains an application diagnostics.
/// </summary>
internal class Diagnostics
{
    /// <summary>
    /// Backing field convention mismatch warning.
    /// </summary>
    public static readonly WarningDescriptor BackingFieldConventionMismatch = new(
        code: "ST01",
        title: "Backing field options mismatch.",
        message: "Backing field PascalCase naming convention cannot be used without underscore.",
        category: "Convention");
}
