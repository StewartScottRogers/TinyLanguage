namespace TinyLanguage.Lexer
{
    // Represents one formal parameter in a function or constructor definition.
    // Four forms are supported:
    //   <id>                        — bare name, no type, no default
    //   <id> ":=" <expr>            — with default value only
    //   <id> ":" <type>             — with type annotation only
    //   <id> ":" <type> ":=" <expr> — with both type and default
    public class ParameterNode : AstNode
    {
        // The parameter name.
        public readonly string ParameterName;

        // Optional declared type — null when no type annotation was provided.
        public readonly TypeNode DeclaredType;

        // Optional default value expression — null when no default was provided.
        public readonly AstNode DefaultExpression;

        public ParameterNode(string parameterName, TypeNode declaredType, AstNode defaultExpression, int line) : base(line)
        {
            ParameterName = parameterName;
            DeclaredType = declaredType;
            DefaultExpression = defaultExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
