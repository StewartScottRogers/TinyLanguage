using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a method definition inside a class body:
//   [static] function id ( param_list? ) (-> type)? stmt_list end
// ReturnType is null when absent. IsStatic is true when the static modifier is present.
public sealed class MethodDefNode : AstNode
{
    public readonly bool IsStatic;
    public readonly string Name;
    public readonly List<ParameterNode> Parameters;
    public readonly TypeNode ReturnType; // null when absent
    public readonly List<AstNode> Body;

    public MethodDefNode(bool isStatic, string name, List<ParameterNode> parameters, TypeNode returnType, List<AstNode> body, int line) : base(line)
    {
        IsStatic = isStatic;
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
