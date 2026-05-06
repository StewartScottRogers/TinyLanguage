using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents an object instantiation expression (Implementation Note 15):
    //   "new" <id> "(" [<arg_list>] ")"
    // This is a valid primary expression and always appears as the right-hand
    // side of an assignment: myObj := new ClassName(args).
    // There is no separate object_declare_stmt in the grammar.
    public class NewExprNode : AstNode
    {
        // The name of the class to instantiate.
        public string ClassId { get; }

        // The ordered list of constructor argument expressions (may be empty).
        public IReadOnlyList<AstNode> Args { get; }

        public NewExprNode(int line, string classId, IReadOnlyList<AstNode> args)
            : base(line)
        {
            ClassId = classId;
            Args = args;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitNewExprNode(this);
        }
    }
}
