using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents the inline conditional expression (only valid in expression contexts):
//   if expr then expr else expr
// Not to be confused with IfStatementNode which is the statement form.
public sealed class ConditionalExprNode : AstNode
{
    public readonly AstNode Condition;
    public readonly AstNode ThenExpr;
    public readonly AstNode ElseExpr;

    public ConditionalExprNode(AstNode condition, AstNode thenExpr, AstNode elseExpr, int line) : base(line)
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
