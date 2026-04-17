using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a type cast expression: ( type ) unary_expr
// The cast applies to the immediately following unary expression,
// so (int) a + b parses as ((int) a) + b.
public sealed class CastExprNode : AstNode
{
    public readonly TypeNode TargetType;
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
