namespace TinyLanguage.Lexer.Nodes
{
    // Represents a string literal value. The lexer strips the surrounding
    // quotes and resolves escape sequences (\n, \t, \\, \", \') before
    // the parser constructs this node.
    public class StringLiteralNode : AstNode
    {
        // The decoded string value (quotes removed, escapes resolved).
        public string Value { get; }

        public StringLiteralNode(int line, string value)
            : base(line)
        {
            Value = value;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitStringLiteralNode(this);
        }
    }
}
