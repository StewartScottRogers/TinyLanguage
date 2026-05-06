namespace TinyLanguage.Lexer.Nodes
{
    // Represents the "null" literal value. Null is falsy in truthiness checks.
    public class NullLiteralNode : AstNode
    {
        public NullLiteralNode(int line) : base(line) { }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitNullLiteralNode(this);
        }
    }
}
