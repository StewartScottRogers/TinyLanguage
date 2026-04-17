using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents: for id := expr to expr [step expr] do stmt_list end
// StepExpr is null when no step clause is present; the default step is 1.
public sealed class ForStatementNode : AstNode
{
    public readonly string VariableName;
    public readonly AstNode StartExpr;
    public readonly AstNode EndExpr;
    public readonly AstNode StepExpr; // null when absent
    public readonly List<AstNode> Body;

    public ForStatementNode(string variableName, AstNode startExpr, AstNode endExpr, AstNode stepExpr, List<AstNode> body, int line) : base(line)
    {
        VariableName = variableName;
        StartExpr = startExpr;
        EndExpr = endExpr;
        StepExpr = stepExpr;
        Body = body;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
