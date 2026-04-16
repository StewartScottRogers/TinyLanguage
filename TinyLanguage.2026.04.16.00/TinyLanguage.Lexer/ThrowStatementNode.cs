namespace TinyLanguage.Lexer
{
    // Represents a throw statement: "throw" <expr>
    // The expression must evaluate to a string at runtime (the exception message).
    public class ThrowStatementNode : AstNode
    {
        // The expression that produces the value to throw.
        public readonly AstNode ThrownExpression;

        public ThrowStatementNode(AstNode thrownExpression, int line) : base(line)
        {
            ThrownExpression = thrownExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
