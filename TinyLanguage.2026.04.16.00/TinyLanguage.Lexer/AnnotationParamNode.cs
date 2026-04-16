namespace TinyLanguage.Lexer
{
    // Represents one key-value parameter in an annotation:
    // <id> "=" <expr>
    // Note: uses "=" (SingleEqual), not ":=".
    public class AnnotationParamNode : AstNode
    {
        // The parameter key name.
        public readonly string KeyName;

        // The parameter value expression.
        public readonly AstNode ValueExpression;

        public AnnotationParamNode(string keyName, AstNode valueExpression, int line) : base(line)
        {
            KeyName = keyName;
            ValueExpression = valueExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
