using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a foreach loop:
    //   "foreach" <id> "in" <expr> "do" <stmt_list> "end"
    // Iterating a string yields each character as a single-character string.
    // Iterating null throws. Iterating a non-list, non-string type throws
    // (Implementation Note 6).
    public class ForeachStatementNode : AstNode
    {
        // The iteration variable bound to each successive element.
        public string Id { get; }

        // The expression that produces the list or string to iterate.
        public AstNode IterableExpr { get; }

        // The statements that form the loop body.
        public IReadOnlyList<AstNode> BodyStatements { get; }

        public ForeachStatementNode(
            int line,
            string id,
            AstNode iterableExpr,
            IReadOnlyList<AstNode> bodyStatements)
            : base(line)
        {
            Id = id;
            IterableExpr = iterableExpr;
            BodyStatements = bodyStatements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitForeachStatementNode(this);
        }
    }
}
