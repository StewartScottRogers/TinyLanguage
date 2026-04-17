using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Root node of the AST. Contains the flat top-level statement list.
public sealed class ProgramNode : AstNode
{
    public readonly List<AstNode> Statements;

    public ProgramNode(List<AstNode> statements, int line) : base(line)
    {
        Statements = statements;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
