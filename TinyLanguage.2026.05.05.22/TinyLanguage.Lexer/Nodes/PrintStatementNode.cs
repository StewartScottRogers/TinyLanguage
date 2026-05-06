namespace TinyLanguage.Lexer.Nodes
{
    // Represents the "print" keyword statement:
    //   "print" <expr>
    // "print" is a keyword statement, not a callable built-in function.
    public class PrintStatementNode : AstNode
    {
        // The expression whose string representation is written to Console.Out.
        public AstNode Expr { get; }

        public PrintStatementNode(int line, AstNode expr)
            : base(line)
        {
            Expr = expr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitPrintStatementNode(this);
        }
    }
}
