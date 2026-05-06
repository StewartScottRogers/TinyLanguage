namespace TinyLanguage.Lexer.Nodes
{
    // Represents the "as" type-assertion expression:
    //   <additive_expr> "as" <type>
    // Performs a checked type assertion — succeeds if the value is of the
    // specified type, throws if it is not. Parsed at comparison precedence
    // level (Implementation Note 11).
    public class TypeAssertNode : AstNode
    {
        // The expression to assert.
        public AstNode Expr { get; }

        // The type to assert the value is.
        public TypeNode AssertType { get; }

        public TypeAssertNode(int line, AstNode expr, TypeNode assertType)
            : base(line)
        {
            Expr = expr;
            AssertType = assertType;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitTypeAssertNode(this);
        }
    }
}
