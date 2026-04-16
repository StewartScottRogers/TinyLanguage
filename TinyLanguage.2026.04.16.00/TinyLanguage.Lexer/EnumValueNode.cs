namespace TinyLanguage.Lexer
{
    // Represents a single member in an enum body.
    // Either a bare identifier or an identifier with an assigned expression.
    // Note: enum bodies use "=" (SingleEqual), not ":=".
    public class EnumValueNode : AstNode
    {
        // The name of this enum member.
        public readonly string MemberName;

        // The optional explicit value expression — null when not specified.
        // When null the interpreter assigns the next sequential integer.
        public readonly AstNode ValueExpression;

        public EnumValueNode(string memberName, AstNode valueExpression, int line) : base(line)
        {
            MemberName = memberName;
            ValueExpression = valueExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
