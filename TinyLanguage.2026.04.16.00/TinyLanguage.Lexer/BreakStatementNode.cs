namespace TinyLanguage.Lexer
{
    // Represents a break statement: "break"
    // Exits the innermost enclosing loop immediately.
    public class BreakStatementNode : AstNode
    {
        public BreakStatementNode(int line) : base(line)
        {
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
