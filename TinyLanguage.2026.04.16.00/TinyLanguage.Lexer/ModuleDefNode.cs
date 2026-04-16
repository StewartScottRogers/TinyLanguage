using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a module definition:
    // "module" <id> "{" <stmt_list> "}"
    // or: "module" <id> "import" <module_import_list> "{" <stmt_list> "}"
    // The header import list uses ModuleImportNode (no double "import" keyword).
    public class ModuleDefNode : AstNode
    {
        // The module name.
        public readonly string ModuleName;

        // The header import list — empty when no "import" clause is present.
        public readonly List<ModuleImportNode> HeaderImports;

        // The module body statements.
        public readonly List<AstNode> Body;

        public ModuleDefNode(string moduleName, List<ModuleImportNode> headerImports, List<AstNode> body, int line) : base(line)
        {
            ModuleName = moduleName;
            HeaderImports = headerImports;
            Body = body;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
