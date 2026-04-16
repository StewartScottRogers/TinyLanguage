namespace TinyLanguage.Lexer
{
    // Represents a return statement: "return" [<expr>]
    // A bare "return" (with no expression) returns null — used in void functions.
    // The expression is optional; ReturnExpression is null for a bare return.
    public class ReturnStatementNode : AstNode
    {
        // The expression to return — null for a bare "return" statement.
        public readonly AstNode ReturnExpression;

        public ReturnStatementNode(AstNode returnExpression, int line) : base(line)
        {
            ReturnExpression = returnExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
