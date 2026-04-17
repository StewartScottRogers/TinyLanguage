using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: try stmt_list catch_clause [finally stmt_list] end
// CatchVariableType is null for the bare "catch id" form.
// FinallyBody is empty when no finally clause is present.
public sealed class TryStatementNode : AstNode
{
    public readonly List<AstNode> TryBody;
    public readonly string CatchVariableName;
    public readonly TypeNode CatchVariableType; // null for bare "catch id" form
    public readonly List<AstNode> CatchBody;
    public readonly List<AstNode> FinallyBody;

    public TryStatementNode(List<AstNode> tryBody, string catchVariableName, TypeNode catchVariableType, List<AstNode> catchBody, List<AstNode> finallyBody, int line) : base(line)
    {
        TryBody = tryBody;
        CatchVariableName = catchVariableName;
        CatchVariableType = catchVariableType;
        CatchBody = catchBody;
        FinallyBody = finallyBody;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
