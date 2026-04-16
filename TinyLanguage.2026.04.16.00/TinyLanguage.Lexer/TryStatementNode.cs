using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a try statement: "try" <stmt_list> <catch_clause> ["finally" <stmt_list>] "end"
    // The finally block is optional — FinallyBody is null when absent.
    public class TryStatementNode : AstNode
    {
        // The list of statements in the protected try block.
        public readonly List<AstNode> TryBody;

        // The catch clause that handles any thrown exception.
        public readonly CatchClauseNode CatchClause;

        // The optional finally block — null when no "finally" clause was present.
        public readonly List<AstNode> FinallyBody;

        public TryStatementNode(List<AstNode> tryBody, CatchClauseNode catchClause, List<AstNode> finallyBody, int line) : base(line)
        {
            TryBody = tryBody;
            CatchClause = catchClause;
            FinallyBody = finallyBody;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
