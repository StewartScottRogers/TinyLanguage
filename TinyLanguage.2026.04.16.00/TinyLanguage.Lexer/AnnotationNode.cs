using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents an annotation: "@" <id> or "@" <id> "(" [<annotation_param_list>] ")"
    // Annotations attach metadata to the following statement.
    // Inside annotation parameter lists "=" (SingleEqual) assigns values — not ":=".
    public class AnnotationNode : AstNode
    {
        // The annotation name (identifier following "@").
        public readonly string AnnotationName;

        // The annotation parameters — empty list when absent or when parentheses are empty.
        public readonly List<AnnotationParamNode> Parameters;

        public AnnotationNode(string annotationName, List<AnnotationParamNode> parameters, int line) : base(line)
        {
            AnnotationName = annotationName;
            Parameters = parameters;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
