using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a single case or default clause inside a switch.
// When IsDefault is true, CaseExpr is null.
public sealed class SwitchCaseNode
{
    public readonly bool IsDefault;
    public readonly AstNode CaseExpr; // null for default clause
    public readonly List<AstNode> Body;

    public SwitchCaseNode(bool isDefault, AstNode caseExpr, List<AstNode> body)
    {
        IsDefault = isDefault;
        CaseExpr = caseExpr;
        Body = body;
    }
}

// Represents: switch expr { case_list }
public sealed class SwitchStatementNode : AstNode
{
    public readonly AstNode Subject;
    public readonly List<SwitchCaseNode> Cases;

    public SwitchStatementNode(AstNode subject, List<SwitchCaseNode> cases, int line) : base(line)
    {
        Subject = subject;
        Cases = cases;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
