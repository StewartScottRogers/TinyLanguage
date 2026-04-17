using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a unary operator expression: op operand
// Operator is the token value string (e.g. "-", "not")
public sealed class UnaryOpNode : AstNode
{
    public readonly string Operator;
    public readonly AstNode Operand;

    public UnaryOpNode(string op, AstNode operand, int line) : base(line)
    {
        Operator = op;
        Operand = operand;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
