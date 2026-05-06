namespace TinyLanguage.Lexer.Nodes
{
    // Represents an integer literal value (decimal, binary 0b, octal 0o,
    // or hexadecimal 0x). The parser converts the token text to a long
    // before constructing this node.
    public class IntegerLiteralNode : AstNode
    {
        // The parsed integer value.
        public long Value { get; }

        public IntegerLiteralNode(int line, long value)
            : base(line)
        {
            Value = value;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitIntegerLiteralNode(this);
        }
    }
}
