namespace TinyLanguage.Lexer
{
    // Represents a continue statement: "continue"
    // Skips the remainder of the current loop body iteration and re-evaluates the condition.
    public class ContinueStatementNode : AstNode
    {
        public ContinueStatementNode(int line) : base(line)
        {
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
