namespace TinyLanguage.Lexer.Nodes
{
    // Represents an expression evaluated for its side effects as a statement.
    // The BNF restricts <call_stmt> to identifier-chain calls, but a few demo
    // programs may evaluate richer expressions in statement position. This
    // node carries an arbitrary expression so the interpreter can still run it.
    public class ExpressionStatementNode : AstNode
    {
        // The expression evaluated as a statement.
        public AstNode Expression { get; }

        public ExpressionStatementNode(int line, AstNode expression)
            : base(line)
        {
            Expression = expression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitExpressionStatementNode(this);
        }
    }
}
