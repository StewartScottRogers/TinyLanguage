namespace TinyLanguage.Lexer
{
    // Represents a type-check expression: <additive_expr> "is" <type>
    // Returns a boolean indicating whether the value is of the specified type.
    // Precedence is at the comparison level (not postfix) — see spec note 11.
    public class TypeCheckNode : AstNode
    {
        // The expression whose runtime type is being tested.
        public readonly AstNode SubjectExpression;

        // The type to check against.
        public readonly TypeNode CheckedType;

        public TypeCheckNode(AstNode subjectExpression, TypeNode checkedType, int line) : base(line)
        {
            SubjectExpression = subjectExpression;
            CheckedType = checkedType;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
