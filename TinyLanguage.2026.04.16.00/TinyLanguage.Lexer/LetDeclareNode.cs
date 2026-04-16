namespace TinyLanguage.Lexer
{
    // Represents a let declaration: "let" <id> ":=" <expr>
    // or the typed form:            "let" <id> ":" <type> ":=" <expr>
    // Creates a new mutable binding in the current (innermost) scope.
    public class LetDeclareNode : AstNode
    {
        // The name of the variable being declared.
        public readonly string VariableName;

        // Optional type annotation — null when no type was specified.
        public readonly TypeNode DeclaredType;

        // The initialiser expression.
        public readonly AstNode InitialiserExpression;

        public LetDeclareNode(string variableName, TypeNode declaredType, AstNode initialiserExpression, int line) : base(line)
        {
            VariableName = variableName;
            DeclaredType = declaredType;
            InitialiserExpression = initialiserExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
