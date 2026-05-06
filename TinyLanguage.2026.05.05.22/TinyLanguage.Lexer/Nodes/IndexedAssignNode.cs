namespace TinyLanguage.Lexer.Nodes
{
    // Represents an index-assignment on a postfix-expression target:
    //   <postfix_expr> "[" <expr> "]" ":=" <expr>
    // Example: this.data[i] := v; matrix[r][c] := 0; obj.list[0] := x
    //
    // ArrayElementAssignNode covers the simple <id> "[" <expr> "]" ":=" <expr>
    // form required by the BNF. This node generalises that to any postfix
    // target (member access, nested index, etc.) which is required by Tier B
    // class demos that store arrays inside objects.
    public class IndexedAssignNode : AstNode
    {
        // The target index-access expression on the LHS of ':='.
        public IndexAccessNode Target { get; }

        // The value expression to store at the given index.
        public AstNode ValueExpr { get; }

        public IndexedAssignNode(int line, IndexAccessNode target, AstNode valueExpr)
            : base(line)
        {
            Target = target;
            ValueExpr = valueExpr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitIndexedAssignNode(this);
        }
    }
}
