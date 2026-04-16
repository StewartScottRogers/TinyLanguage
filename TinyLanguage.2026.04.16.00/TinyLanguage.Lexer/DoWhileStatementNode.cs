using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a do-while loop: "do" <stmt_list> "while" <expr>
    // The body executes at least once; the condition is evaluated after each iteration.
    // Note: there is no trailing "end" — parsing stops at "while" (spec note 3).
    public class DoWhileStatementNode : AstNode
    {
        // The list of statements forming the loop body (executed before the condition check).
        public readonly List<AstNode> Body;

        // The loop condition expression — evaluated after each execution of the body.
        public readonly AstNode Condition;

        public DoWhileStatementNode(List<AstNode> body, AstNode condition, int line) : base(line)
        {
            Body = body;
            Condition = condition;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
