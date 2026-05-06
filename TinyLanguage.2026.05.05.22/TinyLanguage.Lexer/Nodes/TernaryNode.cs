namespace TinyLanguage.Lexer.Nodes
{
    // Represents a ternary (conditional) operator expression:
    //   <or_expr> "?" <expr> ":" <expr>
    // Right-associative at the lowest expression precedence level.
    public class TernaryNode : AstNode
    {
        // The condition to evaluate.
        public AstNode Condition { get; }

        // The expression evaluated when the condition is truthy.
        public AstNode ThenExpr { get; }

        // The expression evaluated when the condition is falsy.
        public AstNode ElseExpr { get; }

        public TernaryNode(int line, AstNode condition, AstNode thenExpr, AstNode elseExpr)
            : base(line)
        {
            Condition = condition;
            ThenExpr = thenExpr;
            ElseExpr = elseExpr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitTernaryNode(this);
        }
    }
}
