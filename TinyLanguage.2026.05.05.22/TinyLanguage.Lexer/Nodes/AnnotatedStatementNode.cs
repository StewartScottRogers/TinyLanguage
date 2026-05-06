namespace TinyLanguage.Lexer.Nodes
{
    // Represents an annotated statement:
    //   <annotation> <stmt>
    // The annotation is parsed and carried along but the wrapped statement
    // executes normally. Silent no-ops are forbidden; the wrapped statement
    // must execute.
    public class AnnotatedStatementNode : AstNode
    {
        // The annotation applied to the statement.
        public AnnotationNode Annotation { get; }

        // The statement being annotated.
        public AstNode WrappedStatement { get; }

        public AnnotatedStatementNode(int line, AnnotationNode annotation, AstNode wrappedStatement)
            : base(line)
        {
            Annotation = annotation;
            WrappedStatement = wrappedStatement;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitAnnotatedStatementNode(this);
        }
    }
}
