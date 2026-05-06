namespace TinyLanguage.Lexer.Nodes
{
    // Represents one import entry in a module's header import list
    // (without the "import" keyword — used inside "module Foo import ..."):
    //   <id>
    //   <id> "as" <id>
    // This is distinct from ImportStatementNode which is a standalone statement.
    public class ModuleImportNode : AstNode
    {
        // The name of the module being imported.
        public string Id { get; }

        // The optional local alias for the imported module. Null when "as" is absent.
        public string AliasId { get; }

        public ModuleImportNode(int line, string id, string aliasId)
            : base(line)
        {
            Id = id;
            AliasId = aliasId;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitModuleImportNode(this);
        }
    }
}
