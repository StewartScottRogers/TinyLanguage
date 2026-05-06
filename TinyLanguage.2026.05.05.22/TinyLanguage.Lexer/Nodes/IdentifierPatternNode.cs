namespace TinyLanguage.Lexer.Nodes
{
    // A pattern that matches any value and binds it to a named variable:
    //   <id>
    // When used in a match case, the identifier becomes a local binding
    // holding the matched value for use in the case body.
    public class IdentifierPatternNode : PatternNode
    {
        // The name of the binding variable.
        public string Id { get; }

        public IdentifierPatternNode(int line, string id) : base(line)
        {
            Id = id;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitIdentifierPatternNode(this);
        }
    }
}
