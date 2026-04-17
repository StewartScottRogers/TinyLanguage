using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents the continue statement — skips to the next loop iteration.
public sealed class ContinueStatementNode : AstNode
{
    public ContinueStatementNode(int line) : base(line)
    {
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
