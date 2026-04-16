namespace TinyLanguage.Lexer
{
    // Represents a boolean literal: "true" or "false".
    public class BoolLiteralNode : AstNode
    {
        // The boolean value.
        public readonly bool Value;

        public BoolLiteralNode(bool value, int line) : base(line)
        {
            Value = value;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
