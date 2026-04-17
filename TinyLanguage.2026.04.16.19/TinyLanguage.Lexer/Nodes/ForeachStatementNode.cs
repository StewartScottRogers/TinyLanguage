using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: foreach id in expr do stmt_list end
// Iterates lists (element by element) and strings (character by character).
public sealed class ForeachStatementNode : AstNode
{
    public readonly string VariableName;
    public readonly AstNode Collection;
    public readonly List<AstNode> Body;

    public ForeachStatementNode(string variableName, AstNode collection, List<AstNode> body, int line) : base(line)
    {
        VariableName = variableName;
        Collection = collection;
        Body = body;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
