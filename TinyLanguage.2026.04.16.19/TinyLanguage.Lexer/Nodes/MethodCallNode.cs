using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents postfix method call: expr . id ( arg_list? )
// The target is the object (any expression) and MethodName is the called member.
public sealed class MethodCallNode : AstNode
{
    public readonly AstNode Target;
    public readonly string MethodName;
    public readonly List<AstNode> Arguments;

    public MethodCallNode(AstNode target, string methodName, List<AstNode> arguments, int line) : base(line)
    {
        Target = target;
        MethodName = methodName;
        Arguments = arguments;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
