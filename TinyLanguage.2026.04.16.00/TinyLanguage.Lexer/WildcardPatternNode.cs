namespace TinyLanguage.Lexer
{
    // Represents the wildcard pattern "_" in a match expression.
    // Matches any value unconditionally.
    public class WildcardPatternNode : AstNode
    {
        public WildcardPatternNode(int line) : base(line)
        {
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
