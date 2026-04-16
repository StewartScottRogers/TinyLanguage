using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a function or method call used as a standalone statement.
    // BNF: <id> { "." <id> } "(" [<arg_list>] ")"
    // Examples: foo(), obj.method(x), a.b.c(x, y)
    // See spec note 22 for restrictions on the call statement form.
    public class CallStatementNode : AstNode
    {
        // The receiver chain: one or more identifiers joined by ".".
        // For a plain call "foo()", this list contains just "foo".
        // For "obj.method()", this list contains ["obj", "method"].
        public readonly List<string> ReceiverChain;

        // The argument expressions passed to the call.
        public readonly List<AstNode> Arguments;

        public CallStatementNode(List<string> receiverChain, List<AstNode> arguments, int line) : base(line)
        {
            ReceiverChain = receiverChain;
            Arguments = arguments;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
