using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a plain assignment: id := expr
public sealed class AssignStatementNode : AstNode
{
    public readonly string Name;
    public readonly AstNode Value;

    public AssignStatementNode(string name, AstNode value, int line) : base(line)
    {
        Name = name;
        Value = value;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
