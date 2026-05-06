using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a switch statement:
    //   "switch" <expr> "{" <case_list> "}"
    // The case list contains CaseClauseNode and/or DefaultClauseNode items.
    public class SwitchStatementNode : AstNode
    {
        // The expression whose value is compared against each case.
        public AstNode SwitchExpr { get; }

        // The ordered list of case and default clauses.
        public IReadOnlyList<AstNode> Cases { get; }

        public SwitchStatementNode(int line, AstNode switchExpr, IReadOnlyList<AstNode> cases)
            : base(line)
        {
            SwitchExpr = switchExpr;
            Cases = cases;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitSwitchStatementNode(this);
        }
    }
}
