using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents an if statement: "if" <expr> "then" <stmt_list> ["else" <stmt_list>] "end"
    // The else branch is optional — ElseBody is null when absent.
    public class IfStatementNode : AstNode
    {
        // The Boolean condition expression.
        public readonly AstNode Condition;

        // The list of statements to execute when the condition is true.
        public readonly List<AstNode> ThenBody;

        // The list of statements to execute when the condition is false.
        // Null when no else branch was present.
        public readonly List<AstNode> ElseBody;

        public IfStatementNode(AstNode condition, List<AstNode> thenBody, List<AstNode> elseBody, int line) : base(line)
        {
            Condition = condition;
            ThenBody = thenBody;
            ElseBody = elseBody;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
