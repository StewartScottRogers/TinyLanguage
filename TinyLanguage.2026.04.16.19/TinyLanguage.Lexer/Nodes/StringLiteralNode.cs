using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a string literal (double-quoted or single-quoted).
// Value is the decoded string content (escape sequences already resolved).
public sealed class StringLiteralNode : AstNode
{
    public readonly string Value;

    public StringLiteralNode(string value, int line) : base(line)
    {
        Value = value;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
