namespace TinyLanguage.Lexer.Nodes
{
    // Represents a "const" declaration: creates an immutable binding.
    //   const <id> ":=" <expr>
    //   const <id> ":" <type> ":=" <expr>
    // Note: the "enum" form of const_declare_stmt uses EnumDefNode instead.
    public class ConstDeclareNode : AstNode
    {
        // The name of the constant.
        public string Id { get; }

        // Optional type annotation. Null when the untyped form is used.
        public TypeNode DeclaredType { get; }

        // The initialiser expression.
        public AstNode Expr { get; }

        public ConstDeclareNode(int line, string id, TypeNode declaredType, AstNode expr)
            : base(line)
        {
            Id = id;
            DeclaredType = declaredType;
            Expr = expr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitConstDeclareNode(this);
        }
    }
}
