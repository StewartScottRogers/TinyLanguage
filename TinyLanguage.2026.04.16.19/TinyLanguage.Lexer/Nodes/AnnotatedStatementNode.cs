using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: annotation stmt
// The annotation wraps the inner statement. At runtime the inner statement
// executes normally; annotations are passed through without side effects.
public sealed class AnnotatedStatementNode : AstNode
{
    public readonly AnnotationNode Annotation;
    public readonly AstNode Statement;

    public AnnotatedStatementNode(AnnotationNode annotation, AstNode statement, int line) : base(line)
    {
        Annotation = annotation;
        Statement = statement;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
