namespace TinyLanguage.Lexer
{
    // Represents an index access (subscript) expression: <expr> "[" <expr> "]"
    // Used for reading array elements by index. Left-associative as a postfix expression.
    public class IndexAccessNode : AstNode
    {
        // The expression that produces the array (or list) to index into.
        public readonly AstNode TargetExpression;

        // The expression that produces the index value.
        public readonly AstNode IndexExpression;

        public IndexAccessNode(AstNode targetExpression, AstNode indexExpression, int line) : base(line)
        {
            TargetExpression = targetExpression;
            IndexExpression = indexExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
