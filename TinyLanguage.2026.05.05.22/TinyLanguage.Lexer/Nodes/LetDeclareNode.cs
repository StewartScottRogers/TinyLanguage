namespace TinyLanguage.Lexer.Nodes
{
    // Represents a "let" declaration: creates a new mutable variable binding.
    //   let <id> ":=" <expr>
    //   let <id> ":" <type> ":=" <expr>
    public class LetDeclareNode : AstNode
    {
        // The name of the new variable.
        public string Id { get; }

        // Optional type annotation. Null when the "let x := expr" form is used.
        public TypeNode DeclaredType { get; }

        // The initialiser expression.
        public AstNode Expr { get; }

        public LetDeclareNode(int line, string id, TypeNode declaredType, AstNode expr)
            : base(line)
        {
            Id = id;
            DeclaredType = declaredType;
            Expr = expr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitLetDeclareNode(this);
        }
    }
}
