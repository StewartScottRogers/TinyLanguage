namespace TinyLanguage.Lexer
{
    // Represents a list comprehension expression: "[" <expr> "for" <id> "in" <expr> "]"
    // Builds a new list by evaluating the element expression for each item in the source list.
    public class ListComprehensionNode : AstNode
    {
        // The expression that produces each element of the output list.
        public readonly AstNode ElementExpression;

        // The name of the loop variable that receives each element from the source.
        public readonly string LoopVariable;

        // The expression that produces the source list to iterate over.
        public readonly AstNode SourceExpression;

        public ListComprehensionNode(AstNode elementExpression, string loopVariable, AstNode sourceExpression, int line) : base(line)
        {
            ElementExpression = elementExpression;
            LoopVariable = loopVariable;
            SourceExpression = sourceExpression;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
