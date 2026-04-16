using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a single case inside a match expression:
    // <pattern> "=>" <stmt_list>
    // or: <pattern> "when" <expr> "=>" <stmt_list>
    public class PatternCaseNode : AstNode
    {
        // The pattern to test against the match subject.
        public readonly AstNode Pattern;

        // Optional guard expression — null when no "when" clause is present.
        public readonly AstNode WhenGuard;

        // The list of statements to execute when the pattern matches.
        public readonly List<AstNode> Body;

        public PatternCaseNode(AstNode pattern, AstNode whenGuard, List<AstNode> body, int line) : base(line)
        {
            Pattern = pattern;
            WhenGuard = whenGuard;
            Body = body;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
