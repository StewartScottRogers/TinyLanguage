using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: let id := expr   or   let id : type := expr
// TypeAnnotation is null when no type annotation is present.
public sealed class LetDeclareNode : AstNode
{
    public readonly string Name;
    public readonly TypeNode TypeAnnotation; // null when absent
    public readonly AstNode Value;

    public LetDeclareNode(string name, TypeNode typeAnnotation, AstNode value, int line) : base(line)
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
