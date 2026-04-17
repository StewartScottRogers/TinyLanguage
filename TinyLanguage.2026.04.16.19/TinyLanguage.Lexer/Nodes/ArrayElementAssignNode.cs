using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: id [ expr ] := expr
// This is a structurally distinct node from AssignStatementNode.
// It mutates the existing list object in-place (does not rebind the variable).
public sealed class ArrayElementAssignNode : AstNode
{
    public readonly string ArrayName;
    public readonly AstNode IndexExpr;
    public readonly AstNode Value;

    public ArrayElementAssignNode(string arrayName, AstNode indexExpr, AstNode value, int line) : base(line)
    {
        ArrayName = arrayName;
        IndexExpr = indexExpr;
        Value = value;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
