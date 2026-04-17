using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: const id := expr   or   const id : type := expr
// TypeAnnotation is null when absent.
public sealed class ConstDeclareNode : AstNode
{
    public readonly string Name;
    public readonly TypeNode TypeAnnotation; // null when absent
    public readonly AstNode Value;

    public ConstDeclareNode(string name, TypeNode typeAnnotation, AstNode value, int line) : base(line)
    {
        Name = name;
        TypeAnnotation = typeAnnotation;
        Value = value;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
