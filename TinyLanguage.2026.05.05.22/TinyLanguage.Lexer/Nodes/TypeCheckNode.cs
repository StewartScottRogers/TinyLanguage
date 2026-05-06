namespace TinyLanguage.Lexer.Nodes
{
    // Represents the "is" type-check expression:
    //   <additive_expr> "is" <type>
    // Returns a Boolean indicating whether the left-hand value is of the
    // specified type. Parsed at comparison precedence level (Implementation Note 11).
    public class TypeCheckNode : AstNode
    {
        // The expression whose runtime type is checked.
        public AstNode Expr { get; }

        // The type to check against.
        public TypeNode CheckType { get; }

        public TypeCheckNode(int line, AstNode expr, TypeNode checkType)
            : base(line)
        {
            Expr = expr;
            CheckType = checkType;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitTypeCheckNode(this);
        }
    }
}
