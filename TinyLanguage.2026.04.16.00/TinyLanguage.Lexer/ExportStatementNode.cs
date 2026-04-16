namespace TinyLanguage.Lexer
{
    // Represents an export statement: "export" <id>
    // Makes a name visible to other modules that import this module.
    public class ExportStatementNode : AstNode
    {
        // The name of the identifier to export.
        public readonly string ExportedName;

        public ExportStatementNode(string exportedName, int line) : base(line)
        {
            ExportedName = exportedName;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
