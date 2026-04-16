namespace TinyLanguage.Lexer
{
    // Represents the null literal: "null"
    // Evaluates to the absence of a value at runtime.
    public class NullLiteralNode : AstNode
    {
        public NullLiteralNode(int line) : base(line)
        {
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
