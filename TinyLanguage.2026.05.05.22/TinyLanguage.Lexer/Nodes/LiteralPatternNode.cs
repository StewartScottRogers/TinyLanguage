namespace TinyLanguage.Lexer.Nodes
{
    // A pattern that matches when the matched value equals a specific literal:
    //   <number> | <string> | <boolean> | "null"
    public class LiteralPatternNode : PatternNode
    {
        // The literal value node (IntegerLiteralNode, FloatLiteralNode,
        // StringLiteralNode, BoolLiteralNode, or NullLiteralNode).
        public AstNode LiteralExpr { get; }

        public LiteralPatternNode(int line, AstNode literalExpr) : base(line)
        {
            LiteralExpr = literalExpr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitLiteralPatternNode(this);
        }
    }
}
