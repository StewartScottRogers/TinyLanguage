namespace TinyLanguage.Lexer.Nodes
{
    // Represents a binary operator expression such as a + b, a && b, a == b, etc.
    // The Op string holds the operator text (e.g. "+", "&&", "==", "//", "**").
    public class BinaryOpNode : AstNode
    {
        // The operator as a string, e.g. "+", "-", "*", "/", "%", "//", "**",
        // "&", "==", "!=", "<", ">", "<=", ">=", "&&", "||", "and", "or".
        public string Op { get; }

        // The left operand.
        public AstNode Left { get; }

        // The right operand.
        public AstNode Right { get; }

        public BinaryOpNode(int line, string op, AstNode left, AstNode right)
            : base(line)
        {
            Op = op;
            Left = left;
            Right = right;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitBinaryOpNode(this);
        }
    }
}
