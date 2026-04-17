using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents a list comprehension: [ expr for id in expr ]
// Produces a new list by evaluating ProjectionExpr for each element
// of the SourceExpr collection, binding each element to VariableName.
public sealed class ListComprehensionNode : AstNode
{
    public readonly AstNode ProjectionExpr;
    public readonly string VariableName;
    public readonly AstNode SourceExpr;

    public ListComprehensionNode(AstNode projectionExpr, string variableName, AstNode sourceExpr, int line) : base(line)
    {
        ProjectionExpr = projectionExpr;
        VariableName = variableName;
        SourceExpr = sourceExpr;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
