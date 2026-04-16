using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Interpreter
{
    // Represents a class definition at runtime.
    // Stores member definitions (fields, methods, constructor) and the scope
    // in which the class was defined.
    public sealed class ClassValue
    {
        // The class name.
        public readonly string Name;

        // The member AST nodes from the class body.
        public readonly List<AstNode> Members;

        // The scope in which the class was defined.
        public readonly Scope DefinitionScope;

        // The name of the base class — null when no "extends" clause.
        public readonly string BaseClassName;

        public ClassValue(string name, List<AstNode> members, Scope definitionScope, string baseClassName)
        {
            Name = name;
            Members = members;
            DefinitionScope = definitionScope;
            BaseClassName = baseClassName;
        }

        public override string ToString()
        {
            return $"<class {Name}>";
        }
    }
}
