namespace TinyLanguage.Lexer.Nodes
{
    // Represents a list comprehension expression:
    //   "[" <expr> "for" <id> "in" <expr> "]"
    // Per the spec, list comprehension is parsed but not yet implemented.
    // The interpreter must throw "Feature not yet implemented: list comprehension"
    // when this node is evaluated.
    public class ListComprehensionNode : AstNode
    {
        // The element expression evaluated for each iteration variable value.
        public AstNode ElementExpr { get; }

        // The iteration variable name.
        public string IterationVar { get; }

        // The expression that produces the list to iterate over.
        public AstNode IterableExpr { get; }

        public ListComprehensionNode(int line, AstNode elementExpr, string iterationVar, AstNode iterableExpr)
            : base(line)
        {
            ElementExpr = elementExpr;
            IterationVar = iterationVar;
            IterableExpr = iterableExpr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitListComprehensionNode(this);
        }
    }
}
