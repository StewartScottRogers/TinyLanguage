namespace TinyLanguage.Lexer.Nodes
{
    // Represents index access on an array or string:
    //   <postfix_expr> "[" <expr> "]"
    // Example: arr[i], matrix[0][1].
    public class IndexAccessNode : AstNode
    {
        // The expression producing the array or string.
        public AstNode Target { get; }

        // The index expression.
        public AstNode IndexExpr { get; }

        public IndexAccessNode(int line, AstNode target, AstNode indexExpr)
            : base(line)
        {
            Target = target;
            IndexExpr = indexExpr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitIndexAccessNode(this);
        }
    }
}
