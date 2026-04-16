using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a function call expression: <id> "(" [<arg_list>] ")"
    // or a call via a stored value: <expr> "(" [<arg_list>] ")"
    // This covers both named function calls and calls through variables holding lambdas.
    public class FunctionCallNode : AstNode
    {
        // The expression that produces the callable value (usually an IdentifierNode).
        public readonly AstNode CalleeExpression;

        // The argument expressions passed to the function.
        public readonly List<AstNode> Arguments;

        public FunctionCallNode(AstNode calleeExpression, List<AstNode> arguments, int line) : base(line)
        {
            CalleeExpression = calleeExpression;
            Arguments = arguments;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
