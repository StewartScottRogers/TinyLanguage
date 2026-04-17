using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents an integer literal: decimal, binary (0b), octal (0o), or hex (0x).
// Value is the parsed 64-bit signed integer.
public sealed class IntegerLiteralNode : AstNode
{
    public readonly long Value;

    public IntegerLiteralNode(long value, int line) : base(line)
    {
        Value = value;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
