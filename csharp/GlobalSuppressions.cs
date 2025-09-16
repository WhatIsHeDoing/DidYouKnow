using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design",
    "CA2210:AssembliesShouldHaveValidStrongNames")]

[assembly: SuppressMessage("Microsoft.Performance",
    "CA1811:AvoidUncalledPrivateCode",
    Scope = "namespace",
    Target = "csharp")]

[assembly: SuppressMessage("Microsoft.Performance",
    "CA1812:AvoidUninstantiatedInternalClasses",
    Scope = "type",
    Target = "csharp.Main+class")]

[assembly: SuppressMessage("xUnit",
    "xUnit1031",
    Justification = "Parallelisation is disabled")]
