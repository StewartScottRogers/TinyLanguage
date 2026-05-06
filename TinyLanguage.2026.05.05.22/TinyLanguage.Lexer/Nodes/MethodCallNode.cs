using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a method call in expression context:
    //   <postfix_expr> "." <id> "(" [<arg_list>] ")"
    // Example: obj.method(a, b).
    public class MethodCallNode : AstNode
    {
        // The expression producing the object on which the method is called.
        public AstNode Target { get; }

        // The method name.
        public string MethodId { get; }

        // The ordered list of argument expressions (may be empty).
        public IReadOnlyList<AstNode> Args { get; }

        public MethodCallNode(int line, AstNode target, string methodId, IReadOnlyList<AstNode> args)
            : base(line)
        {
            Target = target;
            MethodId = methodId;
            Args = args;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitMethodCallNode(this);
        }
    }
}
