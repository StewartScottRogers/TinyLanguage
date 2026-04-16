namespace TinyLanguage.Lexer
{
    // Represents a standalone import statement:
    // "import" <id>           — import by name
    // "import" <id> "as" <id> — import with a local alias
    public class ImportStatementNode : AstNode
    {
        // The name of the module being imported.
        public readonly string ModuleName;

        // The local alias — null when no "as" alias was specified.
        public readonly string Alias;

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
}
