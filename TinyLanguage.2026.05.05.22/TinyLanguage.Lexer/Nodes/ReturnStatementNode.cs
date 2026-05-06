namespace TinyLanguage.Lexer.Nodes
{
    // Represents a return statement:
    //   "return" [<expr>]
    // A bare "return" (no expression) returns null, per Implementation Note 4.
    public class ReturnStatementNode : AstNode
    {
        // The return value expression. Null for bare "return" (void return).
        public AstNode ReturnExpr { get; }

        public ReturnStatementNode(int line, AstNode returnExpr)
            : base(line)
        {
            ReturnExpr = returnExpr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitReturnStatementNode(this);
        }
    }
}
