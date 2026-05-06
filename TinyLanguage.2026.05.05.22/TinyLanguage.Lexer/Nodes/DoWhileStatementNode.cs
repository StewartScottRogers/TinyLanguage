using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a do-while loop:
    //   "do" <stmt_list> "while" <expr>
    // There is no trailing "end" — the "while" keyword terminates the body
    // (Implementation Note 3).  The body always executes at least once.
    public class DoWhileStatementNode : AstNode
    {
        // The statements that form the loop body.
        public IReadOnlyList<AstNode> BodyStatements { get; }

        // The condition evaluated after each iteration.
        public AstNode Condition { get; }

        public DoWhileStatementNode(int line, IReadOnlyList<AstNode> bodyStatements, AstNode condition)
            : base(line)
        {
            BodyStatements = bodyStatements;
            Condition = condition;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitDoWhileStatementNode(this);
        }
    }
}
