using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a postfix call on any expression that evaluates to a callable:
    //   <postfix_expr> "(" [<arg_list>] ")"
    // This is the "value-call" form — calling a function stored in a variable
    // or returned from another expression. Named function calls via <id> are
    // also parsed through this node after the identifier is resolved as an
    // IdentifierNode in the callee position.
    public class FunctionCallNode : AstNode
    {
        // The expression that produces the function/lambda to call.
        public AstNode CalleeExpr { get; }

        // The ordered list of argument expressions (may be empty).
        public IReadOnlyList<AstNode> Args { get; }

        public FunctionCallNode(int line, AstNode calleeExpr, IReadOnlyList<AstNode> args)
            : base(line)
        {
            CalleeExpr = calleeExpr;
            Args = args;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitFunctionCallNode(this);
        }
    }
}
