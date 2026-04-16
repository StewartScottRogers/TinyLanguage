using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a while loop: "while" <expr> "do" <stmt_list> "end"
    // The body is executed repeatedly as long as the condition is truthy.
    public class WhileStatementNode : AstNode
    {
        // The loop condition expression — evaluated before each iteration.
        public readonly AstNode Condition;

        // The list of statements forming the loop body.
        public readonly List<AstNode> Body;

        public WhileStatementNode(AstNode condition, List<AstNode> body, int line) : base(line)
        {
            Condition = condition;
            Body = body;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
