namespace TinyLanguage.Lexer.Nodes
{
    // Represents array element assignment (Implementation Note 7):
    //   <id> "[" <expr> "]" ":=" <expr>
    // This is structurally distinct from AssignStatementNode and must
    // mutate the existing list object in-place rather than rebinding the
    // variable. It must NOT be encoded inside AssignStatementNode.
    public class ArrayElementAssignNode : AstNode
    {
        // The name of the array variable being mutated.
        public string Id { get; }

        // The index expression (value between the brackets).
        public AstNode IndexExpr { get; }

        // The value expression to store at the given index.
        public AstNode ValueExpr { get; }

        public ArrayElementAssignNode(int line, string id, AstNode indexExpr, AstNode valueExpr)
            : base(line)
        {
            Id = id;
            IndexExpr = indexExpr;
            ValueExpr = valueExpr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitArrayElementAssignNode(this);
        }
    }
}
