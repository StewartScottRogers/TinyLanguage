using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents an if statement (not an inline conditional expression):
    //   "if" <expr> "then" <stmt_list> ["else" <stmt_list>] "end"
    // See Implementation Note 12 for the distinction between if-statement
    // and the inline ConditionalExprNode.
    public class IfStatementNode : AstNode
    {
        // The boolean condition to test.
        public AstNode Condition { get; }

        // Statements executed when the condition is truthy.
        public IReadOnlyList<AstNode> ThenStatements { get; }

        // Statements executed when the condition is falsy.
        // Empty (not null) when no "else" branch is present.
        public IReadOnlyList<AstNode> ElseStatements { get; }

        public IfStatementNode(
            int line,
            AstNode condition,
            IReadOnlyList<AstNode> thenStatements,
            IReadOnlyList<AstNode> elseStatements)
            : base(line)
        {
            Condition = condition;
            ThenStatements = thenStatements;
            ElseStatements = elseStatements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitIfStatementNode(this);
        }
    }
}
