using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents member assignment: <expr> "." <id> ":=" <expr>
    // e.g. obj.Field := value  or  a.b.c := value
    public class MemberAssignNode : AstNode
    {
        // The object expression (may be a chain like a.b).
        public readonly AstNode Target;

        // The member (field) name being assigned.
        public readonly string MemberName;

        // The value to assign.
        public readonly AstNode ValueExpression;

        public MemberAssignNode(AstNode target, string memberName, AstNode valueExpression, int line) : base(line)
        {
            Target = target;
            MemberName = memberName;
            ValueExpression = valueExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
