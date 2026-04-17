using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a single annotation parameter: name = expr
public sealed class AnnotationParam
{
    public readonly string Name;
    public readonly AstNode Value;

    public AnnotationParam(string name, AstNode value)
    {
        Name = name;
        Value = value;
    }
}

// Represents an annotation:
//   @id
//   @id()
//   @id( param_list )
// Parameters is empty when no parentheses or empty parentheses are present.
public sealed class AnnotationNode : AstNode
{
    public readonly string Name;
    public readonly List<AnnotationParam> Parameters;

    public AnnotationNode(string name, List<AnnotationParam> parameters, int line) : base(line)
    {
        Name = name;
        Parameters = parameters;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
