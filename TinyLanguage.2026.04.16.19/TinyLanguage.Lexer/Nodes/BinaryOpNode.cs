using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a binary operator expression: left op right
// Operator is the token value string (e.g. "+", "-", "*", "//", "**", "==", etc.)
public sealed class BinaryOpNode : AstNode
{
    public readonly AstNode Left;
    public readonly string Operator;
    public readonly AstNode Right;

    public BinaryOpNode(AstNode left, string op, AstNode right, int line) : base(line)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
