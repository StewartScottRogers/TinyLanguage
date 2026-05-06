namespace TinyLanguage.Lexer.Nodes
{
    // A pattern that matches any value without binding it:
    //   "_"
    // Used as a catch-all case in a match statement.
    public class WildcardPatternNode : PatternNode
    {
        public WildcardPatternNode(int line) : base(line) { }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitWildcardPatternNode(this);
        }
    }
}
