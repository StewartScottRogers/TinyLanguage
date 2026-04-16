namespace TinyLanguage.Lexer
{
    // Represents a pattern alternation: <pattern> "|" <pattern>
    // Left-associative: A | B | C parses as (A | B) | C (spec note 23).
    // Matches if either the left or right sub-pattern matches.
    public class AlternationPatternNode : AstNode
    {
        // The left sub-pattern.
        public readonly AstNode LeftPattern;

        // The right sub-pattern.
        public readonly AstNode RightPattern;

        public AlternationPatternNode(AstNode leftPattern, AstNode rightPattern, int line) : base(line)
        {
            LeftPattern = leftPattern;
            RightPattern = rightPattern;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
