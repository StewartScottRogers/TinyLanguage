using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a match statement: "match" <expr> "{" <pattern_case_list> "}"
    // Tests the subject expression against each pattern in order.
    public class PatternMatchNode : AstNode
    {
        // The expression whose value is tested against each pattern.
        public readonly AstNode SubjectExpression;

        // The ordered list of pattern cases to try.
        public readonly List<PatternCaseNode> Cases;

        public PatternMatchNode(AstNode subjectExpression, List<PatternCaseNode> cases, int line) : base(line)
        {
            SubjectExpression = subjectExpression;
            Cases = cases;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
