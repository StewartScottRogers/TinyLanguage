namespace TinyLanguage.Lexer.Nodes
{
    // Represents a cast expression (Implementation Note 13):
    //   "(" <type> ")" <unary_expr>
    // The cast applies to the immediately following unary expression, giving
    // casts high precedence: "(int) a + b" parses as "((int) a) + b".
    public class CastExprNode : AstNode
    {
        // The target type to cast to.
        public TypeNode TargetType { get; }

        // The expression whose value is cast.
        public AstNode Expr { get; }

        public CastExprNode(int line, TypeNode targetType, AstNode expr)
            : base(line)
        {
            TargetType = targetType;
            Expr = expr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitCastExprNode(this);
        }
    }
}
