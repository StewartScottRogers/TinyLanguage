namespace TinyLanguage.Lexer.Nodes
{
    // Represents a "continue" statement that skips the remainder of the
    // current loop body and proceeds to the next iteration.
    public class ContinueStatementNode : AstNode
    {
        public ContinueStatementNode(int line) : base(line) { }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitContinueStatementNode(this);
        }
    }
}
