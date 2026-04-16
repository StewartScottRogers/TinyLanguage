namespace TinyLanguage.Lexer
{
    // Represents a cast expression: "(" <type> ")" <unary_expr>
    // Casts have high precedence — (int) a + b parses as ((int) a) + b.
    // See spec note 13 for disambiguation from a grouped expression.
    public class CastExprNode : AstNode
    {
        // The target type to cast to.
        public readonly TypeNode TargetType;

        // The operand expression being cast.
        public readonly AstNode Operand;

        public CastExprNode(TypeNode targetType, AstNode operand, int line) : base(line)
        {
            TargetType = targetType;
            Operand = operand;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
