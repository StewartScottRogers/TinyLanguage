namespace TinyLanguage.Lexer.Nodes
{
    // Abstract base class for all pattern forms used in match statements.
    // Concrete subclasses: IdentifierPatternNode, LiteralPatternNode,
    // WildcardPatternNode, ConstructorPatternNode, ArrayPatternNode,
    // FieldPatternNode, AlternationPatternNode.
    public abstract class PatternNode : AstNode
    {
        protected PatternNode(int line) : base(line) { }
    }
}
