namespace TinyLanguage.Lexer.Nodes
{
    // Represents a "break" statement that exits the innermost loop or switch.
    public class BreakStatementNode : AstNode
    {
        public BreakStatementNode(int line) : base(line) { }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitBreakStatementNode(this);
        }
    }
}
