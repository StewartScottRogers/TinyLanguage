namespace TinyLanguage.Lexer
{
    // Represents a field declaration inside a class body.
    // Five forms are supported (spec grammar for <field_declare_stmt>):
    //   let <id> := <expr>
    //   let <id> : <type> := <expr>
    //   var <id> : <type> := <expr>
    //   const <id> := <expr>
    //   const <id> : <type> := <expr>
    public class FieldDeclareNode : AstNode
    {
        // The keyword that introduced this field: "let", "var", or "const".
        public readonly string Keyword;

        // The field name.
        public readonly string FieldName;

        // Optional type annotation — null when not specified.
        public readonly TypeNode DeclaredType;

        // The initialiser expression.
        public readonly AstNode InitialiserExpression;

        public FieldDeclareNode(string keyword, string fieldName, TypeNode declaredType, AstNode initialiserExpression, int line) : base(line)
        {
            Keyword = keyword;
            FieldName = fieldName;
            DeclaredType = declaredType;
            InitialiserExpression = initialiserExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
