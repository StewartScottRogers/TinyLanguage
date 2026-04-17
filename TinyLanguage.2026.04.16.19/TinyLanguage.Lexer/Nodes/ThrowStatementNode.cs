using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: throw expr
// Raises a runtime exception with the value of the expression as the message.
public sealed class ThrowStatementNode : AstNode
{
    public readonly AstNode Value;

    public ThrowStatementNode(AstNode value, int line) : base(line)
    {
        Value = value;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
