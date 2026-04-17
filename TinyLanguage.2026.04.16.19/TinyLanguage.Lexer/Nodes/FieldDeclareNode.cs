using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a field declaration inside a class body.
// Keyword is "let", "var", or "const".
// TypeAnnotation is null when absent (only valid for let and const without type).
public sealed class FieldDeclareNode : AstNode
{
    public readonly string Keyword; // "let", "var", or "const"
    public readonly string Name;
    public readonly TypeNode TypeAnnotation; // null when absent
    public readonly AstNode Value;

    public FieldDeclareNode(string keyword, string name, TypeNode typeAnnotation, AstNode value, int line) : base(line)
    {
        Keyword = keyword;
        Name = name;
        TypeAnnotation = typeAnnotation;
        Value = value;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
