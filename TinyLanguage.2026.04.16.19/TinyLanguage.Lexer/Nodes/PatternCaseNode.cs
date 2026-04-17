using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a single arm in a match expression:
//   pattern [when expr] => stmt_list
// Guard is null when no "when" clause is present.
public sealed class PatternCaseNode : AstNode
{
    public readonly PatternNode Pattern;
    public readonly AstNode Guard; // null when no "when" clause
    public readonly List<AstNode> Body;

    public PatternCaseNode(PatternNode pattern, AstNode guard, List<AstNode> body, int line) : base(line)
    {
        Pattern = pattern;
        Guard = guard;
        Body = body;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
