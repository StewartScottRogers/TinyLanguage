using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a while loop:
    //   "while" <expr> "do" <stmt_list> "end"
    public class WhileStatementNode : AstNode
    {
        // The loop condition evaluated before each iteration.
        public AstNode Condition { get; }

        // The statements that form the loop body.
        public IReadOnlyList<AstNode> BodyStatements { get; }

        public WhileStatementNode(int line, AstNode condition, IReadOnlyList<AstNode> bodyStatements)
            : base(line)
        {
            Condition = condition;
            BodyStatements = bodyStatements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitWhileStatementNode(this);
        }
    }
}
