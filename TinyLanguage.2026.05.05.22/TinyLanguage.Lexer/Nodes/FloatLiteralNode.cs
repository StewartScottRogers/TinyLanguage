namespace TinyLanguage.Lexer.Nodes
{
    // Represents a floating-point literal value.
    // The parser converts the token text to a double before constructing
    // this node. Both digits before and after the decimal point are required
    // (e.g. "1.5" is valid; ".5" and "1." are not).
    public class FloatLiteralNode : AstNode
    {
        // The parsed floating-point value.
        public double Value { get; }

        public FloatLiteralNode(int line, double value)
            : base(line)
        {
            Value = value;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitFloatLiteralNode(this);
        }
    }
}
