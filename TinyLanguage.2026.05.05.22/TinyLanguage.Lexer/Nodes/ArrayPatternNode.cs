using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // A pattern that matches an array and destructures its elements:
    //   "[" [<pattern_list>] "]"
    // Example: [x, y] matches a two-element array and binds x and y.
    // An empty "[]" pattern matches an empty array.
    public class ArrayPatternNode : PatternNode
    {
        // The sub-patterns for each expected array element (may be empty).
        public IReadOnlyList<PatternNode> ElementPatterns { get; }

        public ArrayPatternNode(int line, IReadOnlyList<PatternNode> elementPatterns) : base(line)
        {
            ElementPatterns = elementPatterns;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitArrayPatternNode(this);
        }
    }
}
