using System;
using System.Collections.Generic;

namespace TinyLanguage.Interpreter
{
    // Represents a built-in function (len, str, int, bool) that is
    // implemented in C# rather than in TinyLanguage source code.
    public sealed class BuiltInFunction
    {
        // The function name for diagnostics.
        public readonly string Name;

        // The expected number of arguments.
        public readonly int Arity;

        // The C# delegate that implements the function.
        // Takes the argument list and the source line number (for error reporting).
        public readonly Func<List<object>, int, object> Implementation;

        public BuiltInFunction(string name, int arity, Func<List<object>, int, object> implementation)
        {
            Name = name;
            Arity = arity;
            Implementation = implementation;
        }

        public override string ToString()
        {
            return $"<built-in {Name}>";
        }
    }
}
