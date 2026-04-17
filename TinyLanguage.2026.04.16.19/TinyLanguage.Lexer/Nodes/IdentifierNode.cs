using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a reference to a named variable or function in an expression.
public sealed class IdentifierNode : AstNode
{
    public readonly string Name;

    public IdentifierNode(string name, int line) : base(line)
    {
        Name = name;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
