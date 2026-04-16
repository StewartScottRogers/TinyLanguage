using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a field pattern in a match expression: <id> "{" [<field_pattern_list>] "}"
    // Matches an object of the named type whose named fields satisfy the given sub-patterns.
    public class FieldPatternNode : AstNode
    {
        // The type name to match against.
        public readonly string TypeName;

        // The list of named field sub-patterns — may be empty.
        public readonly List<FieldPatternEntryNode> FieldPatterns;

        public FieldPatternNode(string typeName, List<FieldPatternEntryNode> fieldPatterns, int line) : base(line)
        {
            TypeName = typeName;
            FieldPatterns = fieldPatterns;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
