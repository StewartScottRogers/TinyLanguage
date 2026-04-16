namespace TinyLanguage.Lexer
{
    // Represents a ternary conditional expression: <cond> "?" <then> ":" <else>
    // This is a value expression (not a statement). It is right-associative.
    // Note: also used for the inline "if <expr> then <expr> else <expr>" form (spec note 12).
    public class ConditionalExprNode : AstNode
    {
        // The Boolean condition expression.
        public readonly AstNode Condition;

        // The expression returned when the condition is truthy.
        public readonly AstNode ThenExpression;

        // The expression returned when the condition is falsy.
        public readonly AstNode ElseExpression;

        public ConditionalExprNode(AstNode condition, AstNode thenExpression, AstNode elseExpression, int line) : base(line)
        {
            Condition = condition;
            ThenExpression = thenExpression;
            ElseExpression = elseExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
