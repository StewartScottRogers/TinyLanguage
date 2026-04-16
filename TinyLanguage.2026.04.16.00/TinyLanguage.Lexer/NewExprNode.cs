using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents an object instantiation expression: "new" <id> "(" [<arg_list>] ")"
    // Always appears as the right-hand side of an assignment (spec note 15).
    // There is no separate object_declare_stmt — use assign_stmt with new_expr.
    public class NewExprNode : AstNode
    {
        // The name of the class to instantiate.
        public readonly string ClassName;

        // The constructor argument expressions.
        public readonly List<AstNode> Arguments;

        public NewExprNode(string className, List<AstNode> arguments, int line) : base(line)
        {
            ClassName = className;
            Arguments = arguments;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
