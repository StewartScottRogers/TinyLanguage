namespace TinyLanguage.Lexer
{
    // Represents a simple assignment statement: <id> ":=" <expr>
    // The target must be an identifier that has already been declared.
    // Assignment updates the variable in the scope where it was declared.
    public class AssignStatementNode : AstNode
    {
        // The name of the variable being assigned.
        public readonly string VariableName;

        // The expression whose value is assigned to the variable.
        public readonly AstNode ValueExpression;

        public AssignStatementNode(string variableName, AstNode valueExpression, int line) : base(line)
        {
            VariableName = variableName;
            ValueExpression = valueExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
