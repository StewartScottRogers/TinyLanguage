using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // A pattern that matches a value by its type name and destructures it:
    //   <id> "(" [<pattern_list>] ")"
    // Example: Point(x, y) matches a Point object and binds x and y.
    // Zero-argument form "Foo()" is also valid.
    public class ConstructorPatternNode : PatternNode
    {
        // The type name to match against.
        public string TypeId { get; }

        // The sub-patterns bound to each constructor argument (may be empty).
        public IReadOnlyList<PatternNode> ArgumentPatterns { get; }

        public ConstructorPatternNode(int line, string typeId, IReadOnlyList<PatternNode> argumentPatterns)
            : base(line)
        {
            TypeId = typeId;
            ArgumentPatterns = argumentPatterns;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitConstructorPatternNode(this);
        }
    }
}
