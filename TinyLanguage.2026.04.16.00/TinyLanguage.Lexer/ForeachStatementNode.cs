using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a foreach loop: "foreach" <id> "in" <expr> "do" <stmt_list> "end"
    // Iterates over a list or string. For strings, each iteration yields one character.
    // Iterating null throws at runtime (spec note 6).
    public class ForeachStatementNode : AstNode
    {
        // The name of the loop variable that receives each element.
        public readonly string ElementVariable;

        // The expression that produces the list or string to iterate over.
        public readonly AstNode IterableExpression;

        // The list of statements forming the loop body.
        public readonly List<AstNode> Body;

        public ForeachStatementNode(string elementVariable, AstNode iterableExpression, List<AstNode> body, int line) : base(line)
        {
            ElementVariable = elementVariable;
            IterableExpression = iterableExpression;
            Body = body;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
