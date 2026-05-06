using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a try/catch/finally block:
    //   "try" <stmt_list> <catch_clause> ["finally" <stmt_list>] "end"
    public class TryStatementNode : AstNode
    {
        // The statements inside the try block.
        public IReadOnlyList<AstNode> TryStatements { get; }

        // The catch clause (always required).
        public CatchClauseNode CatchClause { get; }

        // The statements inside the finally block. Empty when "finally" is absent.
        public IReadOnlyList<AstNode> FinallyStatements { get; }

        public TryStatementNode(
            int line,
            IReadOnlyList<AstNode> tryStatements,
            CatchClauseNode catchClause,
            IReadOnlyList<AstNode> finallyStatements)
            : base(line)
        {
            TryStatements = tryStatements;
            CatchClause = catchClause;
            FinallyStatements = finallyStatements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitTryStatementNode(this);
        }
    }
}
