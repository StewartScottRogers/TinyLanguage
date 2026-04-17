using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: import id   or   import id as id
// Alias is null when no "as" alias is given.
public sealed class ImportStatementNode : AstNode
{
    public readonly string ModuleName;
    public readonly string Alias; // null when absent

    public ImportStatementNode(string moduleName, string alias, int line) : base(line)
    {
        ModuleName = moduleName;
        Alias = alias;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
