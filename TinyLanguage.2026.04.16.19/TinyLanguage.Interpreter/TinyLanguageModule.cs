using System.Collections.Generic;

namespace TinyLanguage.Interpreter;

// Runtime representation of a module. Holds its own scope plus the set of
// names the module has exported so the embedding scope can query them.
public sealed class TinyLanguageModule
{
    public readonly string Name;
    public readonly Scope ModuleScope;
    public readonly HashSet<string> Exports;

    public TinyLanguageModule(string name, Scope moduleScope)
    {
        Name = name;
        ModuleScope = moduleScope;
        Exports = new HashSet<string>();
    }
}
