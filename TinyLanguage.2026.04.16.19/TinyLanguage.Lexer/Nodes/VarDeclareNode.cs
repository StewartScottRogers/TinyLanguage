using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: var id : type := expr
// The type annotation is mandatory for var declarations.
public sealed class VarDeclareNode : AstNode
{
    public readonly string Name;
    public readonly TypeNode TypeAnnotation;
    public readonly AstNode Value;

    public VarDeclareNode(string name, TypeNode typeAnnotation, AstNode value, int line) : base(line)
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
