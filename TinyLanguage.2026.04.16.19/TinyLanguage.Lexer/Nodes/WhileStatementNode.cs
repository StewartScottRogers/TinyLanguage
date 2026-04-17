using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: while expr do stmt_list end
public sealed class WhileStatementNode : AstNode
{
    public readonly AstNode Condition;
    public readonly List<AstNode> Body;

    public WhileStatementNode(AstNode condition, List<AstNode> body, int line) : base(line)
    {
        Condition = condition;
        Body = body;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
