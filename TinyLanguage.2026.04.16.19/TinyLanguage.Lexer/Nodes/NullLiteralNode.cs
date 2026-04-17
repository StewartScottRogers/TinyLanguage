using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents the null literal.
public sealed class NullLiteralNode : AstNode
{
    public NullLiteralNode(int line) : base(line)
    {
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
