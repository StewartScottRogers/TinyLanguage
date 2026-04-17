using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: Constructor ( param_list? ) stmt_list end
// Appears inside a class body only.
public sealed class ConstructorDefNode : AstNode
{
    public readonly List<ParameterNode> Parameters;
    public readonly List<AstNode> Body;

    public ConstructorDefNode(List<ParameterNode> parameters, List<AstNode> body, int line) : base(line)
    {
        Parameters = parameters;
        Body = body;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
