using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: enum Id { member, member = expr, ... }
// Each member is a (name, optional value expression) pair.
public sealed class EnumMember
{
    public readonly string Name;
    public readonly AstNode Value; // null when no explicit value

    public EnumMember(string name, AstNode value)
    {
        Name = name;
        Value = value;
    }
}

public sealed class EnumDefNode : AstNode
{
    public readonly string Name;
    public readonly List<EnumMember> Members;

    public EnumDefNode(string name, List<EnumMember> members, int line) : base(line)
    {
        Name = name;
        Members = members;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
