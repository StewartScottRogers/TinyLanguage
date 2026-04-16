namespace TinyLanguage.Lexer
{
    // Represents a print statement: "print" <expr>
    // print is a keyword statement — it is not a callable built-in function.
    // The expression value is converted to a string and written to Console.Out.
    public class PrintStatementNode : AstNode
    {
        // The expression whose string representation is printed.
        public readonly AstNode Expression;

        public PrintStatementNode(AstNode expression, int line) : base(line)
        {
            Expression = expression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
