using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a class definition:
    // ["static"] "class" <id> ["extends" <id>] ["implements" <id_list>] "{" <member_list> "}"
    // Empty class bodies are valid (spec note, <member_list> has an epsilon alternative).
    public class ClassDefNode : AstNode
    {
        // The class name.
        public readonly string ClassName;

        // The name of the base class — null when no "extends" clause is present.
        public readonly string BaseClassName;

        // The list of interface names in the "implements" clause — empty when absent.
        public readonly List<string> ImplementedInterfaces;

        // The list of member declarations (fields, methods, constructors, consts).
        public readonly List<AstNode> Members;

        // True when the "static" modifier preceded the "class" keyword.
        public readonly bool IsStatic;

        public ClassDefNode(string className, string baseClassName, List<string> implementedInterfaces, List<AstNode> members, bool isStatic, int line) : base(line)
        {
            ClassName = className;
            BaseClassName = baseClassName;
            ImplementedInterfaces = implementedInterfaces;
            Members = members;
            IsStatic = isStatic;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
