using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents an array literal expression: "[" [<expr_list>] "]"
    // An empty array literal "[]" is valid (Elements will be an empty list).
    public class ArrayLiteralNode : AstNode
    {
        // The ordered list of element expressions.
        public readonly List<AstNode> Elements;

        public ArrayLiteralNode(List<AstNode> elements, int line) : base(line)
        {
            Elements = elements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
