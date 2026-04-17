using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a lambda (anonymous function) expression:
//   function ( param_list? ) (-> type)? expr           — expression body
//   function ( param_list? ) (-> type)? stmt_list end  — block body
//
// IsBlockBody is true when the block form (ending with "end") was parsed.
// When IsBlockBody is false, Body contains a single expression node.
// ReturnType is null when no return-type annotation is present.
public sealed class LambdaExprNode : AstNode
{
    public readonly List<ParameterNode> Parameters;
    public readonly TypeNode ReturnType; // null when absent
    public readonly List<AstNode> Body;
    public readonly bool IsBlockBody;

    public LambdaExprNode(List<ParameterNode> parameters, TypeNode returnType, List<AstNode> body, bool isBlockBody, int line) : base(line)
    {
        Parameters = parameters;
        ReturnType = returnType;
        Body = body;
        IsBlockBody = isBlockBody;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
