namespace TinyLanguage.Lexer
{
    // Represents a var declaration: "var" <id> ":" <type> ":=" <expr>
    // The type annotation is mandatory for var declarations.
    // Creates a new mutable binding in the current (innermost) scope.
    public class VarDeclareNode : AstNode
    {
        // The name of the variable being declared.
        public readonly string VariableName;

        // The mandatory type annotation for var declarations.
        public readonly TypeNode DeclaredType;

        // The initialiser expression.
        public readonly AstNode InitialiserExpression;

        public VarDeclareNode(string variableName, TypeNode declaredType, AstNode initialiserExpression, int line) : base(line)
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
