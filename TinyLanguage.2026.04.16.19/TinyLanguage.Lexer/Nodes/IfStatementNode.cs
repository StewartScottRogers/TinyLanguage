using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: if expr then stmt_list [else stmt_list] end
// ElseBody is an empty list when no else branch is present.
public sealed class IfStatementNode : AstNode
{
    public readonly AstNode Condition;
    public readonly List<AstNode> ThenBody;
    public readonly List<AstNode> ElseBody;

    public IfStatementNode(AstNode condition, List<AstNode> thenBody, List<AstNode> elseBody, int line) : base(line)
    {
        Condition = condition;
        ThenBody = thenBody;
        ElseBody = elseBody;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
