namespace TinyLanguage.Lexer.Nodes
{
    // Represents a "var" declaration: creates a new mutable variable binding
    // that always requires an explicit type annotation.
    //   var <id> ":" <type> ":=" <expr>
    public class VarDeclareNode : AstNode
    {
        // The name of the new variable.
        public string Id { get; }

        // The required type annotation (var always requires a type).
        public TypeNode DeclaredType { get; }

        // The initialiser expression.
        public AstNode Expr { get; }

        public VarDeclareNode(int line, string id, TypeNode declaredType, AstNode expr)
            : base(line)
        {
            Id = id;
            DeclaredType = declaredType;
            Expr = expr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitVarDeclareNode(this);
        }
    }
}
