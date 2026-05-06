namespace TinyLanguage.Lexer.Nodes
{
    // Represents one parameter in a function or method definition.
    // All fields after Id are optional per the BNF:
    //   <param> ::= <id>
    //             | <id> ":=" <expr>
    //             | <id> ":" <type>
    //             | <id> ":" <type> ":=" <expr>
    public class ParameterNode : AstNode
    {
        // The parameter name as written in the source.
        public string Id { get; }

        // Optional type annotation. Null when not provided.
        public TypeNode ParameterType { get; }

        // Optional default value expression. Null when not provided.
        public AstNode DefaultExpr { get; }

        public ParameterNode(int line, string id, TypeNode parameterType, AstNode defaultExpr)
            : base(line)
        {
            Id = id;
            ParameterType = parameterType;
            DefaultExpr = defaultExpr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitParameterNode(this);
        }
    }
}
