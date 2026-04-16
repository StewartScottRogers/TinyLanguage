namespace TinyLanguage.Lexer
{
    // Represents a floating-point literal value.
    // Both digits before and after the decimal point are required (e.g. 1.5, not .5 or 1.).
    // The value is already converted to a double at parse time.
    public class FloatLiteralNode : AstNode
    {
        // The parsed floating-point value.
        public readonly double Value;

        public FloatLiteralNode(double value, int line) : base(line)
        {
            Value = value;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
