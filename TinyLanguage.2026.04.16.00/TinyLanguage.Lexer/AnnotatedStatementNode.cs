namespace TinyLanguage.Lexer
{
    // Represents an annotation applied to a statement: <annotation> <stmt>
    // The annotation is parsed and stored; the wrapped statement executes normally.
    public class AnnotatedStatementNode : AstNode
    {
        // The annotation that decorates the statement.
        public readonly AnnotationNode Annotation;

        // The statement that follows the annotation.
        public readonly AstNode InnerStatement;

        public AnnotatedStatementNode(AnnotationNode annotation, AstNode innerStatement, int line) : base(line)
        {
            Annotation = annotation;
            InnerStatement = innerStatement;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
