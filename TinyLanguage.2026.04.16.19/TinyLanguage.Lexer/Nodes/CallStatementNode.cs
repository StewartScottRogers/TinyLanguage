using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a function or method call used as a statement:
//   id { "." id } "(" arg_list? ")"
// Examples: foo(), obj.method(x), a.b.c(x, y)
// MemberPath holds the chain of identifiers before the final call.
// The last element in MemberPath is the method/function name being called.
public sealed class CallStatementNode : AstNode
{
    public readonly List<string> MemberPath; // e.g. ["a", "b", "c"] for a.b.c(...)
    public readonly List<AstNode> Arguments;

    public CallStatementNode(List<string> memberPath, List<AstNode> arguments, int line) : base(line)
    {
        MemberPath = memberPath;
        Arguments = arguments;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
