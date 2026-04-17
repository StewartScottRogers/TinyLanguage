using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: condition ? thenExpr : elseExpr
// Right-associative; lowest operator precedence.
public sealed class TernaryNode : AstNode
{
    public readonly AstNode Condition;
    public readonly AstNode ThenExpr;
    public readonly AstNode ElseExpr;

    public TernaryNode(AstNode condition, AstNode thenExpr, AstNode elseExpr, int line) : base(line)
    {
        Condition = condition;
        ThenExpr = thenExpr;
        ElseExpr = elseExpr;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
