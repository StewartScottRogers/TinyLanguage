namespace TinyLanguage.Lexer
{
    // Represents an array element assignment: <id> "[" <expr> "]" ":=" <expr>
    // This is structurally distinct from a plain AssignStatementNode (spec note 7).
    // It mutates the existing list object in-place rather than rebinding the variable.
    public class ArrayElementAssignNode : AstNode
    {
        // The name of the array variable whose element is being updated.
        public readonly string ArrayName;

        // The index expression that selects which element to update.
        public readonly AstNode IndexExpression;

        // The new value expression to store at the selected index.
        public readonly AstNode ValueExpression;

        public ArrayElementAssignNode(string arrayName, AstNode indexExpression, AstNode valueExpression, int line) : base(line)
        {
            ArrayName = arrayName;
            IndexExpression = indexExpression;
            ValueExpression = valueExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
