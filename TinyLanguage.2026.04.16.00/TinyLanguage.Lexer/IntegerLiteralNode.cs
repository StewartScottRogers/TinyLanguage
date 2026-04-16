namespace TinyLanguage.Lexer
{
    // Represents an integer literal value.
    // Supports decimal, binary (0b…), octal (0o…), and hex (0x…) forms.
    // The value is already converted to a long at parse time.
    public class IntegerLiteralNode : AstNode
    {
        // The parsed integer value.
        public readonly long Value;

        public IntegerLiteralNode(long value, int line) : base(line)
        {
            Value = value;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
