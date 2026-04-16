using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a switch statement: "switch" <expr> "{" <case_list> "}"
    // The case list may contain regular cases and at most one default clause.
    public class SwitchStatementNode : AstNode
    {
        // The expression whose value is compared against each case.
        public readonly AstNode SubjectExpression;

        // The ordered list of case and default clauses.
        public readonly List<SwitchCaseNode> Cases;

        public SwitchStatementNode(AstNode subjectExpression, List<SwitchCaseNode> cases, int line) : base(line)
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
