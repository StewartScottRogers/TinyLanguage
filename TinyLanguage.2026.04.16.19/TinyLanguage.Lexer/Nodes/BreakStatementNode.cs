using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents the break statement — exits the innermost loop.
public sealed class BreakStatementNode : AstNode
{
    public BreakStatementNode(int line) : base(line)
    {
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
