using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Interpreter
{
    // Represents a callable function value at runtime.
    // Stores the parameter definitions, body statements, and the closure scope
    // (which for TinyLanguage is always the global scope per the spec).
    public sealed class FunctionValue
    {
        // The formal parameter definitions.
        public readonly List<ParameterNode> Parameters;

        // The body statements to execute when the function is called.
        public readonly List<AstNode> Body;

        // The scope in which the function was defined (global scope for named
        // functions and lambdas — per spec, no closure capture).
        public readonly Scope ClosureScope;

        // The function name for diagnostics — null for anonymous lambdas.
        public readonly string Name;

        public FunctionValue(string name, List<ParameterNode> parameters, List<AstNode> body, Scope closureScope)
        {
            Name = name;
            Parameters = parameters;
            Body = body;
            ClosureScope = closureScope;
        }

        public override string ToString()
        {
            return Name != null ? $"<function {Name}>" : "<lambda>";
        }
    }
}
