using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents object instantiation: new ClassName ( arg_list? )
// Always appears as the right-hand side of an assignment:  myObj := new Foo(args)
public sealed class NewExprNode : AstNode
{
    public readonly string ClassName;
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
