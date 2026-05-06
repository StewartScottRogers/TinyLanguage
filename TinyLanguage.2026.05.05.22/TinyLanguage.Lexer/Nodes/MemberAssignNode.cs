namespace TinyLanguage.Lexer.Nodes
{
    // Represents a member-target assignment statement:
    //   <postfix_expr> "." <id> ":=" <expr>
    // Example: this.size := 0; obj.head := node; cur.next.next := null
    //
    // The BNF defines only <assign_stmt> (id := expr) and <array_assign_stmt>
    // (id[i] := expr). Member-target assignment is a natural extension required
    // by every Tier B class demo, where methods mutate fields via "this.field".
    // The parser produces this node when the LHS of ':=' is a MemberAccessNode.
    public class MemberAssignNode : AstNode
    {
        // The target member-access expression on the LHS of ':='.
        public MemberAccessNode Target { get; }

        // The value expression assigned to the member.
        public AstNode ValueExpr { get; }

        public MemberAssignNode(int line, MemberAccessNode target, AstNode valueExpr)
            : base(line)
        {
            Target = target;
            ValueExpr = valueExpr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitMemberAssignNode(this);
        }
    }
}
