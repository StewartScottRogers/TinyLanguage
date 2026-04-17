using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a single entry in the module import list header.
// Alias is null when no "as" alias is given.
public sealed class ModuleImport
{
    public readonly string ModuleName;
    public readonly string Alias; // null when absent

    public ModuleImport(string moduleName, string alias)
    {
        ModuleName = moduleName;
        Alias = alias;
    }
}

// Represents:
//   module id { stmt_list }
//   module id import import_list { stmt_list }
// Imports list is empty when no import header is present.
public sealed class ModuleDefNode : AstNode
{
    public readonly string Name;
    public readonly List<ModuleImport> Imports;
    public readonly List<AstNode> Body;

    public ModuleDefNode(string name, List<ModuleImport> imports, List<AstNode> body, int line) : base(line)
    {
        Name = name;
        Imports = imports;
        Body = body;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
