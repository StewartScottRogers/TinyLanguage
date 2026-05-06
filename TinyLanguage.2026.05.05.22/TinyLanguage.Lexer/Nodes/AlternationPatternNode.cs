namespace TinyLanguage.Lexer.Nodes
{
    // A pattern that succeeds if either of two sub-patterns matches.
    //   <pattern> "|" <pattern>
    // Left-associative: A | B | C parses as (A | B) | C
    // (Implementation Note 23).
    public class AlternationPatternNode : PatternNode
    {
        // The left sub-pattern (may itself be an AlternationPatternNode).
        public PatternNode Left { get; }

        // The right sub-pattern.
        public PatternNode Right { get; }

        public AlternationPatternNode(int line, PatternNode left, PatternNode right) : base(line)
        {
            Left = left;
            Right = right;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitAlternationPatternNode(this);
        }
    }
}
