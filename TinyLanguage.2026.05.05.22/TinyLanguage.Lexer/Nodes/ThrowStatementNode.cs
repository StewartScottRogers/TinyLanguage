namespace TinyLanguage.Lexer.Nodes
{
    // Represents a throw statement:
    //   "throw" <expr>
    // The expression is evaluated and its string representation (or value)
    // is used as the exception message.
    public class ThrowStatementNode : AstNode
    {
        // The expression that produces the thrown value.
        public AstNode Expr { get; }

        public ThrowStatementNode(int line, AstNode expr)
            : base(line)
        {
            Expr = expr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitThrowStatementNode(this);
        }
    }
}
