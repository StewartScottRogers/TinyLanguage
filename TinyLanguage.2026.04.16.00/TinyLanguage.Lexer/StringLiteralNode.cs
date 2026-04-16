namespace TinyLanguage.Lexer
{
    // Represents a string literal value.
    // Both single-quoted and double-quoted forms are supported.
    // The value is already unescaped (escape sequences like \n, \t are resolved) at parse time.
    public class StringLiteralNode : AstNode
    {
        // The unescaped string value.
        public readonly string Value;

        public StringLiteralNode(string value, int line) : base(line)
        {
            Value = value;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
