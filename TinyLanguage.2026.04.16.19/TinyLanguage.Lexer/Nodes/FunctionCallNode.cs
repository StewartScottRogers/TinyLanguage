using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a named function call in expression position: id ( arg_list? )
// Also used for built-in calls (len, str, int, bool).
// For calls via a stored lambda value, see the postfix call form which
// produces a MethodCallNode or a value-call variant.
public sealed class FunctionCallNode : AstNode
{
    public readonly string Name;
    public readonly List<AstNode> Arguments;

    public FunctionCallNode(string name, List<AstNode> arguments, int line) : base(line)
    {
        Name = name;
        Arguments = arguments;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
