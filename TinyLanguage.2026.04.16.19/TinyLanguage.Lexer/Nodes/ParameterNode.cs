using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents one parameter in a function or method definition:
//   id
//   id := expr          (default value only)
//   id : type           (type annotation only)
//   id : type := expr   (type annotation and default value)
//
// TypeAnnotation is null when no type annotation is present.
// DefaultValue is null when no default value is provided.
public sealed class ParameterNode : AstNode
{
    public readonly string Name;
    public readonly TypeNode TypeAnnotation;  // null when absent
    public readonly AstNode DefaultValue;    // null when absent

    public ParameterNode(string name, TypeNode typeAnnotation, AstNode defaultValue, int line) : base(line)
    {
        Name = name;
        TypeAnnotation = typeAnnotation;
        DefaultValue = defaultValue;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
