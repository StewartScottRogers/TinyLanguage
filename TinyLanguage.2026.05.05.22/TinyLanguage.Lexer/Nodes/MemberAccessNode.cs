namespace TinyLanguage.Lexer.Nodes
{
    // Represents field/property access on an object:
    //   <postfix_expr> "." <id>
    // Example: obj.field, point.X.
    public class MemberAccessNode : AstNode
    {
        // The expression producing the object.
        public AstNode Target { get; }

        // The name of the field or property being accessed.
        public string MemberId { get; }

        public MemberAccessNode(int line, AstNode target, string memberId)
            : base(line)
        {
            Target = target;
            MemberId = memberId;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitMemberAccessNode(this);
        }
    }
}
