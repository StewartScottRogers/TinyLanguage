using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a pattern-match statement:
    //   "match" <expr> "{" <pattern_case_list> "}"
    public class PatternMatchNode : AstNode
    {
        // The expression whose value is matched against each pattern.
        public AstNode MatchExpr { get; }

        // The ordered list of pattern cases.
        public IReadOnlyList<PatternCaseNode> Cases { get; }

        public PatternMatchNode(int line, AstNode matchExpr, IReadOnlyList<PatternCaseNode> cases)
            : base(line)
        {
            MatchExpr = matchExpr;
            Cases = cases;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitPatternMatchNode(this);
        }
    }
}
