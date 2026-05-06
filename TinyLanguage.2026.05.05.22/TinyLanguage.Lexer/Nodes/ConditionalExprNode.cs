namespace TinyLanguage.Lexer.Nodes
{
    // Represents the inline conditional expression (Implementation Note 12):
    //   "if" <expr> "then" <expr> "else" <expr>
    // Valid ONLY inside expression contexts (primary). This is distinct from
    // IfStatementNode which requires "then <stmt_list> ... end".
    public class ConditionalExprNode : AstNode
    {
        // The Boolean condition.
        public AstNode Condition { get; }

        // The value produced when the condition is truthy.
        public AstNode ThenExpr { get; }

        // The value produced when the condition is falsy.
        public AstNode ElseExpr { get; }

        public ConditionalExprNode(int line, AstNode condition, AstNode thenExpr, AstNode elseExpr)
            : base(line)
        {
            Condition = condition;
            ThenExpr = thenExpr;
            ElseExpr = elseExpr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitConditionalExprNode(this);
        }
    }
}
