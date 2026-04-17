using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: expr is type
// Produces a boolean value at runtime.
// Precedence is at comparison level (same as ==, !=, <, etc.).
public sealed class TypeCheckNode : AstNode
{
    public readonly AstNode Operand;
    public readonly TypeNode CheckType;

    public TypeCheckNode(AstNode operand, TypeNode checkType, int line) : base(line)
    {
        Operand = operand;
        CheckType = checkType;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
