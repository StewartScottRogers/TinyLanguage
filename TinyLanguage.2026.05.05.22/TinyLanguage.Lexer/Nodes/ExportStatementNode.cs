namespace TinyLanguage.Lexer.Nodes
{
    // Represents an export statement that makes a name visible outside
    // the enclosing module:
    //   "export" <id>
    public class ExportStatementNode : AstNode
    {
        // The name being exported from the current module.
        public string Id { get; }

        public ExportStatementNode(int line, string id)
            : base(line)
        {
            Id = id;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitExportStatementNode(this);
        }
    }
}
