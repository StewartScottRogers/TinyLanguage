using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a function or method call used as a statement
    // (Implementation Note 22):
    //   <id> { "." <id> } "(" [<arg_list>] ")"
    // Examples: foo(), obj.method(x), a.b.c(x, y).
    // The TargetIds chain contains the identifiers left of the final "()",
    // e.g. ["a", "b", "c"] for a.b.c(args).
    public class CallStatementNode : AstNode
    {
        // The chain of identifiers forming the call target.
        // The last element is the method/function name; preceding elements
        // are the object chain traversed to reach it.
        public IReadOnlyList<string> TargetIds { get; }

        // The ordered list of argument expressions (may be empty).
        public IReadOnlyList<AstNode> Args { get; }

        public CallStatementNode(int line, IReadOnlyList<string> targetIds, IReadOnlyList<AstNode> args)
            : base(line)
        {
            TargetIds = targetIds;
            Args = args;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitCallStatementNode(this);
        }
    }
}
