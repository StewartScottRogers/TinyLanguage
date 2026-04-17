using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents the boolean literals true and false.
public sealed class BoolLiteralNode : AstNode
{
    public readonly bool Value;

    public BoolLiteralNode(bool value, int line) : base(line)
    {
        Value = value;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
