using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a numeric for loop: "for" <id> ":=" <expr> "to" <expr> ["step" <expr>] "do" <stmt_list> "end"
    // The loop variable is implicitly declared in the loop's inner scope (spec note 19).
    // The range is inclusive on both ends. The step defaults to 1 when not specified.
    public class ForStatementNode : AstNode
    {
        // The name of the loop variable (declared fresh in the loop's inner scope).
        public readonly string LoopVariable;

        // The starting value expression.
        public readonly AstNode StartExpression;

        // The ending value expression (inclusive).
        public readonly AstNode EndExpression;

        // The optional step expression — null when not specified (defaults to 1 at runtime).
        public readonly AstNode StepExpression;

        // The list of statements forming the loop body.
        public readonly List<AstNode> Body;

        public ForStatementNode(string loopVariable, AstNode startExpression, AstNode endExpression, AstNode stepExpression, List<AstNode> body, int line) : base(line)
        {
            LoopVariable = loopVariable;
            StartExpression = startExpression;
            EndExpression = endExpression;
            StepExpression = stepExpression;
            Body = body;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
