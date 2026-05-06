namespace TinyLanguage.Lexer.Nodes
{
    // Represents a field declaration inside a class body. Five forms exist:
    //   let <id> ":=" <expr>
    //   let <id> ":" <type> ":=" <expr>
    //   var <id> ":" <type> ":=" <expr>
    //   const <id> ":=" <expr>
    //   const <id> ":" <type> ":=" <expr>
    // The Kind discriminates between let, var, and const.
    public class FieldDeclareNode : AstNode
    {
        // Which declaration keyword was used.
        public FieldKind Kind { get; }

        // The field name.
        public string Id { get; }

        // Optional type annotation. Null when the untyped form is used.
        public TypeNode DeclaredType { get; }

        // The initialiser expression.
        public AstNode Expr { get; }

        public FieldDeclareNode(int line, FieldKind kind, string id, TypeNode declaredType, AstNode expr)
            : base(line)
        {
            Kind = kind;
            Id = id;
            DeclaredType = declaredType;
            Expr = expr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitFieldDeclareNode(this);
        }
    }

    // Discriminates the three declaration keywords valid inside a class body.
    public enum FieldKind
    {
        Let,
        Var,
        Const
    }
}
