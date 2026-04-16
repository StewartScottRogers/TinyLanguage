namespace TinyLanguage.Lexer
{
    // Represents a unary operation: <operator> <operand>
    // Covers unary minus "-" and logical not "not".
    public class UnaryOpNode : AstNode
    {
        // The operator token value: "-" or "not".
        public readonly string Operator;

        // The operand expression.
        public readonly AstNode Operand;

        public UnaryOpNode(string operatorSymbol, AstNode operand, int line) : base(line)
        {
            Operator = operatorSymbol;
            Operand = operand;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
