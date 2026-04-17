using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: expr as type
// Performs a checked type assertion at runtime — throws if the value
// is not of the specified type. Precedence is at comparison level.
public sealed class TypeAssertNode : AstNode
{
    public readonly AstNode Operand;
    public readonly TypeNode AssertType;

    public TypeAssertNode(AstNode operand, TypeNode assertType, int line) : base(line)
    {
        Operand = operand;
        AssertType = assertType;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
