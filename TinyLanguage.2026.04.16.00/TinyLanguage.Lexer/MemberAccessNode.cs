namespace TinyLanguage.Lexer
{
    // Represents a member access expression: <expr> "." <id>
    // Left-associative postfix expression. Used to read a field or property.
    public class MemberAccessNode : AstNode
    {
        // The expression that produces the object to access.
        public readonly AstNode TargetExpression;

        // The name of the member to access.
        public readonly string MemberName;

        public MemberAccessNode(AstNode targetExpression, string memberName, int line) : base(line)
        {
            TargetExpression = targetExpression;
            MemberName = memberName;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
