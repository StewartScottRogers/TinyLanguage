using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: export id
// Exports the named binding from the current module.
public sealed class ExportStatementNode : AstNode
{
    public readonly string Name;

    public ExportStatementNode(string name, int line) : base(line)
    {
        Name = name;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
