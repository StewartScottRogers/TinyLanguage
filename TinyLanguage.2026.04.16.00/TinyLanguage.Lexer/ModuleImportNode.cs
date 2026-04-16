namespace TinyLanguage.Lexer
{
    // Represents a single entry in a module's header import list:
    // <id>            — import by name
    // <id> "as" <id>  — import with an alias
    // This node is used in the module header import list, NOT for standalone import statements.
    public class ModuleImportNode : AstNode
    {
        // The name of the module to import.
        public readonly string ModuleName;

        // The local alias — null when no "as" alias was specified.
        public readonly string Alias;

        public ModuleImportNode(string moduleName, string alias, int line) : base(line)
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
