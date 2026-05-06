namespace TinyLanguage.Lexer.Nodes
{
    // Represents a unary prefix operator expression.
    // Operators: "-" (arithmetic negation), "not" (logical negation).
    public class UnaryOpNode : AstNode
    {
        // The operator as a string: "-" or "not".
        public string Op { get; }

        // The operand expression.
        public AstNode Operand { get; }

        public UnaryOpNode(int line, string op, AstNode operand)
            : base(line)
        {
            Op = op;
            Operand = operand;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitUnaryOpNode(this);
        }
    }
}
