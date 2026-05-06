namespace TinyLanguage.Lexer.Nodes
{
    // Represents the Boolean literals "true" and "false".
    public class BoolLiteralNode : AstNode
    {
        // The parsed boolean value.
        public bool Value { get; }

        public BoolLiteralNode(int line, bool value)
            : base(line)
        {
            Value = value;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitBoolLiteralNode(this);
        }
    }
}
