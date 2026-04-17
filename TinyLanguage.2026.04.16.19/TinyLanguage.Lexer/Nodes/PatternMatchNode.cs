using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: match expr { pattern_case_list }
public sealed class PatternMatchNode : AstNode
{
    public readonly AstNode Subject;
    public readonly List<PatternCaseNode> Cases;

    public PatternMatchNode(AstNode subject, List<PatternCaseNode> cases, int line) : base(line)
    {
        Subject = subject;
        Cases = cases;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
