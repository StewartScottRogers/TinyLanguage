namespace TinyLanguage.Lexer.Nodes
{
    // Represents one key=value parameter inside an annotation:
    //   <id> "=" <expr>
    // Note: annotation parameters use "=" (not ":=") per the BNF and
    // Implementation Note 2.
    public class AnnotationParamNode : AstNode
    {
        // The parameter name.
        public string Id { get; }

        // The parameter value expression.
        public AstNode ValueExpr { get; }

        public AnnotationParamNode(int line, string id, AstNode valueExpr)
            : base(line)
        {
            Id = id;
            ValueExpr = valueExpr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitAnnotationParamNode(this);
        }
    }
}
