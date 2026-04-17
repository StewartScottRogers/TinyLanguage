using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: return [expr]
// Value is null for a bare return (returns null at runtime).
public sealed class ReturnStatementNode : AstNode
{
    public readonly AstNode Value; // null for bare return

    public ReturnStatementNode(AstNode value, int line) : base(line)
    {
        Value = value;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
