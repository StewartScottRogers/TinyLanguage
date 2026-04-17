using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: print expr
// print is a statement keyword, not a callable function.
public sealed class PrintStatementNode : AstNode
{
    public readonly AstNode Expression;

    public PrintStatementNode(AstNode expression, int line) : base(line)
    {
        Expression = expression;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
