using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a numeric for loop:
    //   "for" <id> ":=" <expr> "to" <expr> ["step" <expr>] "do" <stmt_list> "end"
    // The loop variable is implicitly declared in the loop's own inner scope
    // and is not accessible after the loop exits (Implementation Note 19).
    public class ForStatementNode : AstNode
    {
        // The loop variable name (implicitly scoped to the loop body).
        public string Id { get; }

        // The start value (inclusive).
        public AstNode StartExpr { get; }

        // The end value (inclusive).
        public AstNode EndExpr { get; }

        // The optional step expression. Null when "step" is not specified.
        public AstNode StepExpr { get; }

        // The statements that form the loop body.
        public IReadOnlyList<AstNode> BodyStatements { get; }

        public ForStatementNode(
            int line,
            string id,
            AstNode startExpr,
            AstNode endExpr,
            AstNode stepExpr,
            IReadOnlyList<AstNode> bodyStatements)
            : base(line)
        {
            Id = id;
            StartExpr = startExpr;
            EndExpr = endExpr;
            StepExpr = stepExpr;
            BodyStatements = bodyStatements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitForStatementNode(this);
        }
    }
}
