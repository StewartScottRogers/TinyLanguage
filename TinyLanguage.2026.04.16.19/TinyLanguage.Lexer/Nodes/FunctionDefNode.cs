using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a top-level function definition:
//   function id ( param_list? ) (-> type)? stmt_list end
// ReturnType is null when no return-type annotation is present.
public sealed class FunctionDefNode : AstNode
{
    public readonly string Name;
    public readonly List<ParameterNode> Parameters;
    public readonly TypeNode ReturnType; // null when absent
    public readonly List<AstNode> Body;

    public FunctionDefNode(string name, List<ParameterNode> parameters, TypeNode returnType, List<AstNode> body, int line) : base(line)
    {
        Name = name;
        Parameters = parameters;
        ReturnType = returnType;
        Body = body;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
