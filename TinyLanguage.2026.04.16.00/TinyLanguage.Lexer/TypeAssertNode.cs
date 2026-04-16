namespace TinyLanguage.Lexer
{
    // Represents a type-assert expression: <additive_expr> "as" <type>
    // Performs a checked type assertion — throws at runtime if the type does not match.
    // Precedence is at the comparison level (not postfix) — see spec note 11.
    public class TypeAssertNode : AstNode
    {
        // The expression whose value is being asserted to be of the target type.
        public readonly AstNode SubjectExpression;

        // The type that the value is asserted to have.
        public readonly TypeNode AssertedType;

        public TypeAssertNode(AstNode subjectExpression, TypeNode assertedType, int line) : base(line)
        {
            SubjectExpression = subjectExpression;
            AssertedType = assertedType;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
