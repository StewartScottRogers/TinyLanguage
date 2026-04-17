using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents an array literal: [ expr_list? ]
// Elements is empty for an empty array literal [].
public sealed class ArrayLiteralNode : AstNode
{
    public readonly List<AstNode> Elements;

    public ArrayLiteralNode(List<AstNode> elements, int line) : base(line)
    {
        Elements = elements;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
