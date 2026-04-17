using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: input id
// Reads a line from standard input and stores it in the named variable.
public sealed class InputStatementNode : AstNode
{
    public readonly string VariableName;

    public InputStatementNode(string variableName, int line) : base(line)
    {
        VariableName = variableName;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
