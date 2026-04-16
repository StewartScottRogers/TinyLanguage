namespace TinyLanguage.Lexer
{
    // Represents any binary operation: <left> <operator> <right>
    // Covers arithmetic (+, -, *, /, %, **, //), comparison (==, !=, <, >, <=, >=),
    // logical (&&, ||, and, or), and string concatenation (&).
    public class BinaryOpNode : AstNode
    {
        // The left operand expression.
        public readonly AstNode Left;

        // The operator token value (e.g. "+", "**", "and", "||").
        public readonly string Operator;

        // The right operand expression.
        public readonly AstNode Right;

        public BinaryOpNode(AstNode left, string operatorSymbol, AstNode right, int line) : base(line)
        {
            Left = left;
            Operator = operatorSymbol;
            Right = right;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
