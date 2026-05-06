using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents an annotation header:
    //   "@" <id>
    //   "@" <id> "(" [<annotation_param_list>] ")"
    // Empty parentheses "@ann()" are valid (param list may be empty).
    public class AnnotationNode : AstNode
    {
        // The annotation name (the identifier after "@").
        public string Id { get; }

        // The list of key=value parameters. Empty for "@id" and "@id()".
        public IReadOnlyList<AnnotationParamNode> Parameters { get; }

        public AnnotationNode(int line, string id, IReadOnlyList<AnnotationParamNode> parameters)
            : base(line)
        {
            Id = id;
            Parameters = parameters;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitAnnotationNode(this);
        }
    }
}
