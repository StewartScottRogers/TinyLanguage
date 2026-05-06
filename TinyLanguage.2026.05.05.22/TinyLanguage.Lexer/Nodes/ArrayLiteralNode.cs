using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents an array literal:
    //   "[" [<expr_list>] "]"
    // An empty "[]" creates an empty array (the list is empty, not null).
    public class ArrayLiteralNode : AstNode
    {
        // The ordered list of element expressions (may be empty).
        public IReadOnlyList<AstNode> Elements { get; }

        public ArrayLiteralNode(int line, IReadOnlyList<AstNode> elements)
            : base(line)
        {
            Elements = elements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitArrayLiteralNode(this);
        }
    }
}
