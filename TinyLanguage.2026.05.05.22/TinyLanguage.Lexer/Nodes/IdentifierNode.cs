namespace TinyLanguage.Lexer.Nodes
{
    // Represents a reference to a variable or function by name.
    // At this level the name is unresolved; the interpreter looks it up
    // in the scope chain at runtime.
    public class IdentifierNode : AstNode
    {
        // The unresolved identifier name as written in the source.
        public string Name { get; }

        public IdentifierNode(int line, string name)
            : base(line)
        {
            Name = name;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitIdentifierNode(this);
        }
    }
}
