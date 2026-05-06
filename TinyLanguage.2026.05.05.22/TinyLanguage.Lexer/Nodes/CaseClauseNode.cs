using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents one "case" arm inside a switch statement:
    //   "case" <expr> ":" <stmt_list>
    public class CaseClauseNode : AstNode
    {
        // The value expression to match against the switch expression.
        public AstNode MatchExpr { get; }

        // The statements executed when this case matches.
        public IReadOnlyList<AstNode> BodyStatements { get; }

        public CaseClauseNode(int line, AstNode matchExpr, IReadOnlyList<AstNode> bodyStatements)
            : base(line)
        {
            MatchExpr = matchExpr;
            BodyStatements = bodyStatements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitCaseClauseNode(this);
        }
    }
}
