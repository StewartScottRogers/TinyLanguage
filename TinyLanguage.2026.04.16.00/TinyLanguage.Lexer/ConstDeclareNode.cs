namespace TinyLanguage.Lexer
{
    // Represents a const declaration: "const" <id> ":=" <expr>
    // or the typed form:              "const" <id> ":" <type> ":=" <expr>
    // Creates an immutable binding in the current (innermost) scope.
    // Note: enum declarations use EnumDefNode instead.
    public class ConstDeclareNode : AstNode
    {
        // The name of the constant being declared.
        public readonly string ConstantName;

        // Optional type annotation — null when no type was specified.
        public readonly TypeNode DeclaredType;

        // The initialiser expression whose value is bound to the constant.
        public readonly AstNode InitialiserExpression;

        public ConstDeclareNode(string constantName, TypeNode declaredType, AstNode initialiserExpression, int line) : base(line)
        {
            ConstantName = constantName;
            DeclaredType = declaredType;
            InitialiserExpression = initialiserExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
