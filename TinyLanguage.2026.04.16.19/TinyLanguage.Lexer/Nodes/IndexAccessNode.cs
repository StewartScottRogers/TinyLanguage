using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents postfix index access: expr [ expr ]
// Part of the left-associative postfix chain.
public sealed class IndexAccessNode : AstNode
{
    public readonly AstNode Target;
    public readonly AstNode IndexExpr;

    public IndexAccessNode(AstNode target, AstNode indexExpr, int line) : base(line)
    {
        Target = target;
        IndexExpr = indexExpr;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
