using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a floating-point literal: e.g. 3.14
// Value is the parsed 64-bit double.
public sealed class FloatLiteralNode : AstNode
{
    public readonly double Value;

    public FloatLiteralNode(double value, int line) : base(line)
    {
        Value = value;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
