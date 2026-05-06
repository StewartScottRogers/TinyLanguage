using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a module definition:
    //   "module" <id> "{" <stmt_list> "}"
    //   "module" <id> "import" <module_import_list> "{" <stmt_list> "}"
    // The import list uses ModuleImportNode (without a leading "import" keyword)
    // to avoid the double-keyword parse "module Foo import import Bar ...".
    public class ModuleDefNode : AstNode
    {
        // The module name.
        public string Id { get; }

        // The list of module imports declared in the module header.
        // Empty when no "import" clause follows the module name.
        public IReadOnlyList<ModuleImportNode> ImportList { get; }

        // The statements that form the module body.
        public IReadOnlyList<AstNode> BodyStatements { get; }

        public ModuleDefNode(
            int line,
            string id,
            IReadOnlyList<ModuleImportNode> importList,
            IReadOnlyList<AstNode> bodyStatements)
            : base(line)
        {
            Id = id;
            ImportList = importList;
            BodyStatements = bodyStatements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitModuleDefNode(this);
        }
    }
}
