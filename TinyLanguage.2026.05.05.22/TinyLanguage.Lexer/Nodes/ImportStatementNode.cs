namespace TinyLanguage.Lexer.Nodes
{
    // Represents a standalone import statement inside a module body or
    // at the top level:
    //   "import" <id>
    //   "import" <id> "as" <id>
    public class ImportStatementNode : AstNode
    {
        // The name of the module being imported.
        public string Id { get; }

        // The optional local alias. Null when "as" is absent.
        public string AliasId { get; }

        public ImportStatementNode(int line, string id, string aliasId)
            : base(line)
        {
            Id = id;
            AliasId = aliasId;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitImportStatementNode(this);
        }
    }
}
