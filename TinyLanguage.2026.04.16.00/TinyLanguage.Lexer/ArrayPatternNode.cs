using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents an array pattern in a match expression: "[" [<pattern_list>] "]"
    // Matches an array whose elements match the given sub-patterns in order.
    public class ArrayPatternNode : AstNode
    {
        // The ordered list of element patterns — may be empty.
        public readonly List<AstNode> ElementPatterns;

        public ArrayPatternNode(List<AstNode> elementPatterns, int line) : base(line)
        {
            ElementPatterns = elementPatterns;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
