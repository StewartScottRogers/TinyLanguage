using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a constructor pattern in a match expression: <id> "(" [<pattern_list>] ")"
    // Matches a value that was constructed with the named class and whose fields match the sub-patterns.
    public class ConstructorPatternNode : AstNode
    {
        // The constructor (class) name to match against.
        public readonly string ConstructorName;

        // The sub-patterns that match the constructor arguments — may be empty.
        public readonly List<AstNode> SubPatterns;

        public ConstructorPatternNode(string constructorName, List<AstNode> subPatterns, int line) : base(line)
        {
            ConstructorName = constructorName;
            SubPatterns = subPatterns;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
