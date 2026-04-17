using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a do { stmt_list } block without a trailing while condition.
// This is a standalone scoped block (not a do-while loop).
// Distinguished from DoWhileStatementNode by the absence of the "while" keyword.
public sealed class DoBlockNode : AstNode
{
    public readonly List<AstNode> Body;

    public DoBlockNode(List<AstNode> body, int line) : base(line)
    {
        Body = body;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
