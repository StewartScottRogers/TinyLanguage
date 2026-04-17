using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: [static] class id [extends id] [implements id_list] { member_list }
// ExtendsName and ImplementsNames are empty/null when the clauses are absent.
public sealed class ClassDefNode : AstNode
{
    public readonly bool IsStatic;
    public readonly string Name;
    public readonly string ExtendsName; // null when no extends clause
    public readonly List<string> ImplementsNames;
    public readonly List<AstNode> Members;

    public ClassDefNode(bool isStatic, string name, string extendsName, List<string> implementsNames, List<AstNode> members, int line) : base(line)
    {
        IsStatic = isStatic;
        Name = name;
        ExtendsName = extendsName;
        ImplementsNames = implementsNames;
        Members = members;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
