using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a method call expression: <expr> "." <id> "(" [<arg_list>] ")"
    // Left-associative postfix expression. Calls a named method on an object.
    public class MethodCallNode : AstNode
    {
        // The expression that produces the object on which the method is called.
        public readonly AstNode TargetExpression;

        // The name of the method to call.
        public readonly string MethodName;

        // The argument expressions passed to the method.
        public readonly List<AstNode> Arguments;

        public MethodCallNode(AstNode targetExpression, string methodName, List<AstNode> arguments, int line) : base(line)
        {
            TargetExpression = targetExpression;
            MethodName = methodName;
            Arguments = arguments;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
