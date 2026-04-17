using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: do stmt_list while expr
// The body always executes at least once before the condition is checked.
public sealed class DoWhileStatementNode : AstNode
{
    public readonly List<AstNode> Body;
    public readonly AstNode Condition;

    public DoWhileStatementNode(List<AstNode> body, AstNode condition, int line) : base(line)
    {
        Body = body;
        Condition = condition;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
