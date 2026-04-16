using System.Collections.Generic;

namespace TinyLanguage.Interpreter
{
    // Represents a runtime module definition.
    // Modules hold their own scope and a set of exported names.
    public sealed class ModuleValue
    {
        // The module name.
        public readonly string Name;

        // The module's own scope containing all definitions.
        public readonly Scope ModuleScope;

        // The set of names that have been exported from this module.
        public readonly HashSet<string> ExportedNames;

        public ModuleValue(string name, Scope moduleScope, HashSet<string> exportedNames)
        {
            Name = name;
            ModuleScope = moduleScope;
            ExportedNames = exportedNames;
        }

        public override string ToString()
        {
            return $"<module {Name}>";
        }
    }
}
